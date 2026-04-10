namespace ApiGateway.Providers;

public interface IBillingProvider
{
    Task<string> GetAsync(string path);
    Task<string> PostAsync(string path, object? body = null);
    Task DeleteAsync(string path);
}
