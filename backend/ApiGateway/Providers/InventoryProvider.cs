using System.Text;
using System.Text.Json;

namespace ApiGateway.Providers;

public class InventoryProvider : IInventoryProvider
{
    private readonly HttpClient _httpClient;

    public InventoryProvider(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("InventoryClient");
    }

    public async Task<string> GetAsync(string path)
    {
        var response = await _httpClient.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string path, object body)
    {
        var content = Serialize(body);
        var response = await _httpClient.PostAsync(path, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string path, object body)
    {
        var content = Serialize(body);
        var response = await _httpClient.PutAsync(path, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeleteAsync(string path)
    {
        var response = await _httpClient.DeleteAsync(path);
        response.EnsureSuccessStatusCode();
    }

    public async Task PatchAsync(string path, object body)
    {
        var content = Serialize(body);
        var response = await _httpClient.PatchAsync(path, content);
        response.EnsureSuccessStatusCode();
    }

    private static StringContent Serialize(object body) =>
        new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
}
