using Microsoft.Extensions.Logging;

namespace ApiGateway.Providers;

public class InventoryProvider : BaseProvider, IInventoryProvider
{
    public InventoryProvider(IHttpClientFactory factory, ILogger<InventoryProvider> logger) 
        : base(factory, "InventoryClient", logger)
    {
    }

    public Task<string> GetAsync(string path) => InternalGetAsync(path);

    public Task<string> PostAsync(string path, object body) => InternalPostAsync(path, body);

    public Task<string> PutAsync(string path, object body) => InternalPutWithResponseAsync(path, body);

    public Task DeleteAsync(string path) => InternalDeleteAsync(path);

    public Task PatchAsync(string path, object body) => InternalPatchAsync(path, body);
}
