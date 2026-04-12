using System.Text;
using System.Text.Json;

namespace ApiGateway.Providers;

public abstract class BaseProvider
{
    protected readonly HttpClient HttpClient;
    protected readonly JsonSerializerOptions JsonOptions;

    protected BaseProvider(IHttpClientFactory factory, string clientName)
    {
        HttpClient = factory.CreateClient(clientName);
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    protected async Task<T> GetAsync<T>(string path)
    {
        var response = await HttpClient.GetAsync(path);
        await HandleErrorAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions)!;
    }

    protected async Task<string> GetAsync(string path)
    {
        var response = await HttpClient.GetAsync(path);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task<string> PostAsync(string path, object? body = null)
    {
        var content = CreateContent(body);
        var response = await HttpClient.PostAsync(path, content);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task PutAsync(string path, object body)
    {
        var content = CreateContent(body);
        var response = await HttpClient.PutAsync(path, content);
        await HandleErrorAsync(response);
    }

    protected async Task<string> PutWithResponseAsync(string path, object body)
    {
        var content = CreateContent(body);
        var response = await HttpClient.PutAsync(path, content);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task PatchAsync(string path, object body)
    {
        var content = CreateContent(body);
        var response = await HttpClient.PatchAsync(path, content);
        await HandleErrorAsync(response);
    }

    protected async Task DeleteAsync(string path)
    {
        var response = await HttpClient.DeleteAsync(path);
        await HandleErrorAsync(response);
    }

    protected StringContent CreateContent(object? body)
    {
        if (body == null) return new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var json = JsonSerializer.Serialize(body, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    protected async Task HandleErrorAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            var serviceName = GetType().Name.Replace("Provider", "Service");
            Console.WriteLine($"[GATEWAY ERROR] {serviceName} returned {response.StatusCode}: {errorBody}");
            response.EnsureSuccessStatusCode(); 
        }
    }
}
