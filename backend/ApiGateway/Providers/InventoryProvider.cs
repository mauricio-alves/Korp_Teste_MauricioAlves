using Microsoft.Extensions.Logging;

namespace ApiGateway.Providers;

public class InventoryProvider : BaseProvider, IInventoryProvider
{
    public InventoryProvider(IHttpClientFactory factory, ILogger<InventoryProvider> logger) 
        : base(factory, "InventoryClient", logger)
    {
    }

    public Task<string> GetAsync(string path) => base.GetAsync(path);

    public Task<string> PostAsync(string path, object body) => base.PostAsync(path, body);

    public Task<string> PutAsync(string path, object body) => base.PutWithResponseAsync(path, body);

    public Task DeleteAsync(string path) => base.DeleteAsync(path);

    public Task PatchAsync(string path, object body) => base.PatchAsync(path, body);
}
