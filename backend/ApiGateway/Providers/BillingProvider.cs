using Microsoft.Extensions.Logging;

namespace ApiGateway.Providers;

public class BillingProvider : BaseProvider, IBillingProvider
{
    public BillingProvider(IHttpClientFactory factory, ILogger<BillingProvider> logger) 
        : base(factory, "BillingClient", logger)
    {
    }

    public new Task<string> GetAsync(string path) => base.GetAsync(path);

    public new Task<string> PostAsync(string path, object? body = null) => base.PostAsync(path, body);

    public new Task DeleteAsync(string path) => base.DeleteAsync(path);
}
