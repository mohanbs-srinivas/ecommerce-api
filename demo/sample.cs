//Create a C# API service class to interact with the Azure DevOps REST API. The class should support GET, POST, and PATCH requests.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class AzureDevOpsService
{
    private readonly HttpClient _httpClient;
    private readonly string _organization;
    private readonly string _project;
    private readonly string _personalAccessToken;

    public AzureDevOpsService(string organization, string project, string personalAccessToken)
    {
        _organization = organization;
        _project = project;
        _personalAccessToken = personalAccessToken;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://dev.azure.com/{_organization}/{_project}/_apis/")
        };
        var byteArray = Encoding.ASCII.GetBytes($":{_personalAccessToken}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

    public async Task<string> GetAsync(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint with the provided data as JSON content.
    /// </summary>
    /// <param name="endpoint">The URL of the endpoint to which the POST request will be sent.</param>
    /// <param name="data">The object to be serialized into JSON and sent as the request body.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the response content as a string.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP response indicates an unsuccessful status code.
    /// </exception>
    public async Task<string> PostAsync(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PatchAsync(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
        {
            Content = content
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
