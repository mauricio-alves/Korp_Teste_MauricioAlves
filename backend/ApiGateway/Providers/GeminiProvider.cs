using System.Text;
using System.Text.Json;

namespace ApiGateway.Providers;

public interface IGeminiProvider
{
    Task<string> SuggestProductsAsync(string context, string availableProducts);
}

public class GeminiProvider : IGeminiProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiProvider(IHttpClientFactory factory, IConfiguration configuration)
    {
        _httpClient = factory.CreateClient("GeminiClient");
        _configuration = configuration;
    }

    public async Task<string> SuggestProductsAsync(string context, string availableProducts)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("ERRO CRITICO: Korp AI Key está VAZIA ou não configurada no arquivo '.env' host!");
        }

        var prompt = $"Baseado no contexto: '{context}', sugira quais dos seguintes produtos devem ser incluídos em uma nota fiscal e em qual quantidade. Produtos disponíveis: {availableProducts}. Responda em português, de forma concisa.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}",
            content);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
