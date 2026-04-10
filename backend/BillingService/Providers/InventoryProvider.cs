using System.Text;
using System.Text.Json;

namespace BillingService.Providers;

public class InventoryProvider : IInventoryProvider
{
    private readonly HttpClient _httpClient;

    public InventoryProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InventoryClient");
    }

    public async Task DebitBalanceAsync(Guid productId, int quantity)
    {
        var payload = JsonSerializer.Serialize(new { Quantity = quantity });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"/api/products/{productId}/balance/debit", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreditBalanceAsync(Guid productId, int quantity)
    {
        var payload = JsonSerializer.Serialize(new { Quantity = quantity });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"/api/products/{productId}/balance/credit", content);
        response.EnsureSuccessStatusCode();
    }
}
