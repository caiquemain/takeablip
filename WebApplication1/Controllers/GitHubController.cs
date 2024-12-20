using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubRepoListerAPI.Controllers
{
    /// <summary>
    /// Controlador para listar e filtrar repositórios do GitHub da organizacao Takenet.
    /// </summary>
    [ApiController]
    [Route("api/github")]
    public class GitHubController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private List<GitHubRepository> _cachedRepositories = new();
        private DateTime _cacheExpiration;

        public GitHubController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitHubRepoListerAPI", "1.0"));

            // Set the GitHub token from environment variable
            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("GitHub Token not found. Please set it as an environment variable.");
            }
        }

        private const string GitHubApiUrl = "https://api.github.com/orgs/takenet/repos";

        /// <summary>
        /// Lista todos os repositorios disponíveis na organizacao Takenet.
        /// </summary>
        /// <remarks>
        /// Este metodo retorna uma lista completa de todos os repositorios, independentemente da linguagem ou outros filtros.
        /// </remarks>
        /// <response code="200">Repositorios retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os repositorios no GitHub.</response>
        [HttpGet("repositories/listall")]
        public async Task<IActionResult> ListAllRepositories()
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os repositórios do GitHub.");

            return Ok(repositories);
        }

        /// <summary>
        /// Filtra repositórios por linguagem, nome, ordena por data de criacao e limita os resultados.
        /// </summary>
        /// <param name="language">Linguagem de programacao para filtrar (ex.: C#, Java).</param>
        /// <param name="name">Parte do nome do repositorio para buscar.</param>
        /// <param name="sortOrder">Ordem de classificacao: asc (crescente) ou desc (decrescente).</param>
        /// <param name="limit">Número maximo de resultados a serem retornados.</param>
        /// <remarks>
        /// Este método permite filtrar repositorios da organizacao Takenet com base em criterios específicos:
        /// - **Linguagem**: Filtra os repositorios que utilizam a linguagem especificada.
        /// - **Nome**: Retorna repositorios cujo nome contem o texto informado.
        /// - **Ordenacao**: Define se os resultados devem ser ordenados por data de criacao em ordem crescente (asc) ou decrescente (desc).
        /// - **Limite**: Define o numero máximo de repositorios a serem retornados.
        /// </remarks>
        /// <response code="200">Repositorios filtrados retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os repositorios no GitHub.</response>
        [HttpGet("repositories/filter")]
        public async Task<IActionResult> FilterRepositories(
    [FromQuery] string? language,
    [FromQuery] string? name,
    [FromQuery] string? sortOrder = "asc",
    [FromQuery] int? limit = null)
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os repositórios do GitHub.");

            var filteredRepositories = repositories;

            // Filtrar por linguagem
            if (!string.IsNullOrEmpty(language))
            {
                filteredRepositories = filteredRepositories
                    .Where(r => r.Language?.Equals(language, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            // Filtrar por nome
            if (!string.IsNullOrEmpty(name))
            {
                filteredRepositories = filteredRepositories
                    .Where(r => r.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Ordenar por data de criacao
            filteredRepositories = sortOrder?.ToLower() switch
            {
                "desc" => filteredRepositories.OrderByDescending(r => r.CreatedAt).ToList(),
                _ => filteredRepositories.OrderBy(r => r.CreatedAt).ToList()
            };

            // Limitar resultados
            if (limit.HasValue && limit.Value > 0)
            {
                filteredRepositories = filteredRepositories.Take(limit.Value).ToList();
            }

            // Transformar os repositórios para incluir owner_avatar_url como uma única propriedade
            var transformedRepositories = filteredRepositories.Select(r => new
            {
                r.FullName,
                r.Description,
                r.Language,
                r.CreatedAt,
                OwnerAvatarUrl = r.Owner.AvatarUrl 
            }).ToList();

            return Ok(transformedRepositories);
        }

        private async Task<List<GitHubRepository>?> GetCachedRepositories()
        {
            // Verificar se o cache está válido
            if (_cachedRepositories.Any() && DateTime.UtcNow < _cacheExpiration)
            {
                return _cachedRepositories;
            }

            // Atualizar o cache
            var repositories = await FetchRepositoriesFromGitHub();
            if (repositories != null)
            {
                _cachedRepositories = repositories;
                _cacheExpiration = DateTime.UtcNow.AddMinutes(10); // Cache expira em 10 minutos
            }

            return _cachedRepositories;
        }

        private async Task<List<GitHubRepository>?> FetchRepositoriesFromGitHub()
        {
            var response = await _httpClient.GetAsync($"{GitHubApiUrl}?per_page=100");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GitHubRepository>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public class GitHubRepository
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

        public OwnerInfo Owner { get; set; } = new();

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class OwnerInfo
    {
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
