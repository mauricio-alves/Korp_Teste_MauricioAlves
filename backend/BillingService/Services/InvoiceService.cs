using BillingService.DTOs;
using BillingService.Models;
using BillingService.Providers;
using BillingService.Repositories;

namespace BillingService.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IInventoryProvider _inventoryProvider;

    public InvoiceService(IInvoiceRepository repository, IInventoryProvider inventoryProvider)
    {
        _repository = repository;
        _inventoryProvider = inventoryProvider;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _repository.GetAllAsync();
        return invoices.Select(ToDto);
    }

    public async Task<InvoiceDto> GetByIdAsync(Guid id)
    {
        var invoice = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice with id '{id}' not found.");
        return ToDto(invoice);
    }

    public async Task<InvoiceDto> CreateAsync()
    {
        var number = await _repository.GetNextNumberAsync();
        var invoice = new Invoice { Number = number };
        var created = await _repository.CreateAsync(invoice);
        return ToDto(created);
    }

    public async Task<InvoiceDto> AddItemAsync(Guid invoiceId, AddInvoiceItemDto dto)
    {
        var invoice = await _repository.GetByIdAsync(invoiceId)
            ?? throw new KeyNotFoundException($"Invoice with id '{invoiceId}' not found.");

        if (invoice.Status == InvoiceStatus.Closed)
            throw new InvalidOperationException("Cannot add items to a closed invoice.");

        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var item = new InvoiceItem
        {
            InvoiceId = invoiceId,
            ProductId = dto.ProductId,
            ProductCode = dto.ProductCode,
            ProductDescription = dto.ProductDescription,
            Quantity = dto.Quantity
        };

        await _repository.AddItemAsync(item);

        var updated = await _repository.GetByIdAsync(invoiceId);
        return ToDto(updated!);
    }

    public async Task RemoveItemAsync(Guid invoiceId, Guid itemId)
    {
        var invoice = await _repository.GetByIdAsync(invoiceId)
            ?? throw new KeyNotFoundException($"Invoice with id '{invoiceId}' not found.");

        if (invoice.Status == InvoiceStatus.Closed)
            throw new InvalidOperationException("Cannot remove items from a closed invoice.");

        var item = invoice.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException($"Item with id '{itemId}' not found in invoice.");

        await _repository.RemoveItemAsync(item);
    }

    public async Task<InvoiceDto> PrintAsync(Guid invoiceId)
    {
        var invoice = await _repository.GetByIdAsync(invoiceId)
            ?? throw new KeyNotFoundException($"Invoice with id '{invoiceId}' not found.");

        if (invoice.Status == InvoiceStatus.Closed)
            throw new InvalidOperationException("Invoice is already closed.");

        if (!invoice.Items.Any())
            throw new InvalidOperationException("Cannot print an invoice with no items.");

        var debitedItems = new List<InvoiceItem>();

        try
        {
            foreach (var item in invoice.Items)
            {
                await _inventoryProvider.DebitBalanceAsync(item.ProductId, item.Quantity);
                debitedItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            foreach (var item in debitedItems)
            {
                await _inventoryProvider.CreditBalanceAsync(item.ProductId, item.Quantity);
            }
            throw new InvalidOperationException($"Print failed. Rollback executed. Reason: {ex.Message}");
        }

        invoice.Status = InvoiceStatus.Closed;
        invoice.ClosedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(invoice);
        return ToDto(updated);
    }

    private static InvoiceDto ToDto(Invoice i) => new(
        i.Id,
        i.Number,
        i.Status.ToString(),
        i.CreatedAt,
        i.ClosedAt,
        i.Items.Select(item => new InvoiceItemDto(
            item.Id,
            item.ProductId,
            item.ProductCode,
            item.ProductDescription,
            item.Quantity
        )).ToList()
    );
}
