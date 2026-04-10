using BillingService.Models;

namespace BillingService.Repositories;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<int> GetNextNumberAsync();
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task<InvoiceItem> AddItemAsync(InvoiceItem item);
    Task RemoveItemAsync(InvoiceItem item);
}
