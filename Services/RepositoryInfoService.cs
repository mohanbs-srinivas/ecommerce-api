using System.Text.Json;

namespace ecommerce_api.Services
{
    public class RepositoryInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _githubToken;

        public RepositoryInfoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _githubToken = configuration["GITHUB_API_TOKEN"] ?? Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");
            
            // Configure HttpClient for GitHub API
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ecommerce-api");
            
            if (!string.IsNullOrEmpty(_githubToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_githubToken}");
            }
        }

        public async Task<List<string>> GetBranchesAsync(string owner, string repo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/branches");
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();
                var branches = JsonSerializer.Deserialize<JsonElement[]>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return branches?.Select(b => b.GetProperty("name").GetString() ?? "").Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                throw new InvalidOperationException($"Failed to retrieve branches for {owner}/{repo}", ex);
            }
        }

        public async Task<int> GetCommitCountAsync(string owner, string repo)
        {
            try
            {
                // Get commits from main branch with pagination
                var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/commits?sha=main&per_page=1");
                response.EnsureSuccessStatusCode();
                
                // GitHub returns the total count in Link header for pagination
                // For a simple implementation, we'll make a request to get all commits and count them
                // In a production app, you'd parse the Link header for more efficient counting
                var allCommitsResponse = await _httpClient.GetAsync($"repos/{owner}/{repo}/commits?sha=main&per_page=100");
                allCommitsResponse.EnsureSuccessStatusCode();
                
                var jsonString = await allCommitsResponse.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<JsonElement[]>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return commits?.Length ?? 0;
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                throw new InvalidOperationException($"Failed to retrieve commit count for {owner}/{repo}", ex);
            }
        }

        public async Task<List<string>> GetCollaboratorsAsync(string owner, string repo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/collaborators");
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();
                var collaborators = JsonSerializer.Deserialize<JsonElement[]>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return collaborators?.Select(c => c.GetProperty("login").GetString() ?? "").Where(login => !string.IsNullOrEmpty(login)).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                throw new InvalidOperationException($"Failed to retrieve collaborators for {owner}/{repo}", ex);
            }
        }
    }
}