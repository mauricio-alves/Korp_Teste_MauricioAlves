using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ApiGateway.Providers;

public abstract class BaseProvider
{
    protected readonly HttpClient HttpClient;
    protected readonly JsonSerializerOptions JsonOptions;
    protected readonly ILogger Logger;

    protected BaseProvider(IHttpClientFactory factory, string clientName, ILogger logger)
    {
        HttpClient = factory.CreateClient(clientName);
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Logger = logger;
    }

    protected async Task<T> InternalGetAsync<T>(string path)
    {
        using var response = await HttpClient.GetAsync(path);
        await HandleErrorAsync(response);
        var content = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<T>(content, JsonOptions);
        if (result == null)
        {
            throw new InvalidOperationException($"Failed to deserialize response from {path} to type {typeof(T).Name}");
        }
        
        return result;
    }

    protected async Task<string> InternalGetAsync(string path)
    {
        using var response = await HttpClient.GetAsync(path);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task<string> InternalPostAsync(string path, object? body = null)
    {
        var content = CreateContent(body);
        using var response = await HttpClient.PostAsync(path, content);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task InternalPutAsync(string path, object body)
    {
        var content = CreateContent(body);
        using var response = await HttpClient.PutAsync(path, content);
        await HandleErrorAsync(response);
    }

    protected async Task<string> InternalPutWithResponseAsync(string path, object body)
    {
        var content = CreateContent(body);
        using var response = await HttpClient.PutAsync(path, content);
        await HandleErrorAsync(response);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task InternalPatchAsync(string path, object body)
    {
        var content = CreateContent(body);
        using var response = await HttpClient.PatchAsync(path, content);
        await HandleErrorAsync(response);
    }

    protected async Task InternalDeleteAsync(string path)
    {
        using var response = await HttpClient.DeleteAsync(path);
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
            
            Logger.LogError("[GATEWAY ERROR] {ServiceName} returned {StatusCode}: {ErrorBody}", 
                serviceName, response.StatusCode, errorBody);

            var message = $"{serviceName} returned {(int)response.StatusCode} ({response.StatusCode}).";

            if (!string.IsNullOrWhiteSpace(errorBody))
            {
                try 
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(errorBody);
                    if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        var detail = doc.RootElement.TryGetProperty("detail", out var pDetail) ? pDetail.GetString() : null;
                        var title = doc.RootElement.TryGetProperty("title", out var pTitle) ? pTitle.GetString() : null;
                        
                        var technicalDetail = detail ?? title;
                        if (!string.IsNullOrWhiteSpace(technicalDetail))
                        {
                            message = technicalDetail;
                        }
                    }
                }
                catch { /* Ignore parse errors and use generic message */ }
            }

            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
