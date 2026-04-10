using BillingService.DTOs;

namespace BillingService.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto> GetByIdAsync(Guid id);
    Task<InvoiceDto> CreateAsync();
    Task<InvoiceDto> AddItemAsync(Guid invoiceId, AddInvoiceItemDto dto);
    Task RemoveItemAsync(Guid invoiceId, Guid itemId);
    Task<InvoiceDto> PrintAsync(Guid invoiceId);
}
