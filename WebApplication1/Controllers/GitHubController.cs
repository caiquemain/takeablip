using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubRepoListerAPI.Controllers
{
    /// <summary>
    /// Controlador para listar e filtrar repositórios do GitHub da organização Takenet.
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
        }

        private const string GitHubApiUrl = "https://api.github.com/orgs/takenet/repos";

        /// <summary>
        /// Lista todos os repositórios disponíveis na organização Takenet.
        /// </summary>
        /// <remarks>
        /// Este método retorna uma lista completa de todos os repositórios, independentemente da linguagem ou outros filtros.
        /// </remarks>
        /// <response code="200">Repositórios retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os repositórios no GitHub.</response>
        [HttpGet("repositories/listall")]
        public async Task<IActionResult> ListAllRepositories()
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os repositórios do GitHub.");

            return Ok(repositories);
        }

        /// <summary>
        /// Filtra repositórios por linguagem, nome ou ordena por data de criação.
        /// </summary>
        /// <param name="language">Linguagem de programação para filtrar (ex.: C#, Java).</param>
        /// <param name="name">Parte do nome do repositório para buscar.</param>
        /// <param name="sortOrder">Ordem de classificação: asc (crescente) ou desc (decrescente).</param>
        /// <remarks>
        /// Este método permite filtrar repositórios da organização Takenet com base em critérios específicos:
        /// - **Linguagem**: Filtra os repositórios que utilizam a linguagem especificada.
        /// - **Nome**: Retorna repositórios cujo nome contém o texto informado.
        /// - **Ordenação**: Define se os resultados devem ser ordenados por data de criação em ordem crescente (asc) ou decrescente (desc).
        /// </remarks>
        /// <response code="200">Repositórios filtrados retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os repositórios no GitHub.</response>
        [HttpGet("repositories/filter")]
        public async Task<IActionResult> FilterRepositories(
            [FromQuery] string? language,
            [FromQuery] string? name,
            [FromQuery] string? sortOrder = "asc")
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os repositórios do GitHub.");

            var filteredRepositories = repositories;

            // Filtrar por linguagem
            if (!string.IsNullOrEmpty(language))
            {
                filteredRepositories = filteredRepositories.Where(r => r.Language?.Equals(language, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            // Filtrar por nome
            if (!string.IsNullOrEmpty(name))
            {
                filteredRepositories = filteredRepositories.Where(r => r.FullName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Ordenar por data de criação
            filteredRepositories = sortOrder?.ToLower() switch
            {
                "desc" => filteredRepositories.OrderByDescending(r => r.CreatedAt).ToList(),
                _ => filteredRepositories.OrderBy(r => r.CreatedAt).ToList()
            };

            return Ok(filteredRepositories);
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
