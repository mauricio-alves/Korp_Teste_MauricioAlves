using System.Text;
using System.Text.Json;

namespace ApiGateway.Providers;

public class BillingProvider : IBillingProvider
{
    private readonly HttpClient _httpClient;

    public BillingProvider(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("BillingClient");
    }

    public async Task<string> GetAsync(string path)
    {
        var response = await _httpClient.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string path, object? body = null)
    {
        var content = body is not null
            ? new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            : new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(path, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeleteAsync(string path)
    {
        var response = await _httpClient.DeleteAsync(path);
        response.EnsureSuccessStatusCode();
    }
}
