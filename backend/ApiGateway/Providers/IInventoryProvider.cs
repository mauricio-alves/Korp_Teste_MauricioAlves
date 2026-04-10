namespace ApiGateway.Providers;

public interface IInventoryProvider
{
    Task<string> GetAsync(string path);
    Task<string> PostAsync(string path, object body);
    Task<string> PutAsync(string path, object body);
    Task DeleteAsync(string path);
    Task PatchAsync(string path, object body);
}
