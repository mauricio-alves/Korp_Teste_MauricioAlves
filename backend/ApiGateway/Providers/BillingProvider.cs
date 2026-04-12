using Microsoft.Extensions.Logging;

namespace ApiGateway.Providers;

public class BillingProvider : BaseProvider, IBillingProvider
{
    public BillingProvider(IHttpClientFactory factory, ILogger<BillingProvider> logger) 
        : base(factory, "BillingClient", logger)
    {
    }

    public Task<string> GetAsync(string path) => InternalGetAsync(path);

    public Task<string> PostAsync(string path, object? body = null) => InternalPostAsync(path, body);

    public Task DeleteAsync(string path) => InternalDeleteAsync(path);
}
