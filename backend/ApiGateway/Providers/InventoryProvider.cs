namespace ApiGateway.Providers;

public class InventoryProvider : BaseProvider, IInventoryProvider
{
    public InventoryProvider(IHttpClientFactory factory) : base(factory, "InventoryClient")
    {
    }

    public new Task<string> GetAsync(string path) => base.GetAsync(path);

    public new Task<string> PostAsync(string path, object body) => base.PostAsync(path, body);

    public new Task<string> PutAsync(string path, object body) => base.PutWithResponseAsync(path, body);

    public new Task DeleteAsync(string path) => base.DeleteAsync(path);

    public new Task PatchAsync(string path, object body) => base.PatchAsync(path, body);
}
