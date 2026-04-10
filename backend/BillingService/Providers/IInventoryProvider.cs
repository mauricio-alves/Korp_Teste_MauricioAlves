namespace BillingService.Providers;

public interface IInventoryProvider
{
    Task DebitBalanceAsync(Guid productId, int quantity);
    Task CreditBalanceAsync(Guid productId, int quantity);
}
