using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubRepoListerAPI.Controllers
{
    /// <summary>
    /// Controlador para listar e filtrar reposit�rios do GitHub da organiza��o Takenet.
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
        /// Lista todos os reposit�rios dispon�veis na organiza��o Takenet.
        /// </summary>
        /// <remarks>
        /// Este m�todo retorna uma lista completa de todos os reposit�rios, independentemente da linguagem ou outros filtros.
        /// </remarks>
        /// <response code="200">Reposit�rios retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os reposit�rios no GitHub.</response>
        [HttpGet("repositories/listall")]
        public async Task<IActionResult> ListAllRepositories()
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os reposit�rios do GitHub.");

            return Ok(repositories);
        }

        /// <summary>
        /// Filtra reposit�rios por linguagem, nome ou ordena por data de cria��o.
        /// </summary>
        /// <param name="language">Linguagem de programa��o para filtrar (ex.: C#, Java).</param>
        /// <param name="name">Parte do nome do reposit�rio para buscar.</param>
        /// <param name="sortOrder">Ordem de classifica��o: asc (crescente) ou desc (decrescente).</param>
        /// <remarks>
        /// Este m�todo permite filtrar reposit�rios da organiza��o Takenet com base em crit�rios espec�ficos:
        /// - **Linguagem**: Filtra os reposit�rios que utilizam a linguagem especificada.
        /// - **Nome**: Retorna reposit�rios cujo nome cont�m o texto informado.
        /// - **Ordena��o**: Define se os resultados devem ser ordenados por data de cria��o em ordem crescente (asc) ou decrescente (desc).
        /// </remarks>
        /// <response code="200">Reposit�rios filtrados retornados com sucesso.</response>
        /// <response code="500">Erro ao buscar os reposit�rios no GitHub.</response>
        [HttpGet("repositories/filter")]
        public async Task<IActionResult> FilterRepositories(
            [FromQuery] string? language,
            [FromQuery] string? name,
            [FromQuery] string? sortOrder = "asc")
        {
            var repositories = await GetCachedRepositories();
            if (repositories == null) return StatusCode(500, "Erro ao buscar os reposit�rios do GitHub.");

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

            // Ordenar por data de cria��o
            filteredRepositories = sortOrder?.ToLower() switch
            {
                "desc" => filteredRepositories.OrderByDescending(r => r.CreatedAt).ToList(),
                _ => filteredRepositories.OrderBy(r => r.CreatedAt).ToList()
            };

            return Ok(filteredRepositories);
        }

        private async Task<List<GitHubRepository>?> GetCachedRepositories()
        {
            // Verificar se o cache est� v�lido
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
