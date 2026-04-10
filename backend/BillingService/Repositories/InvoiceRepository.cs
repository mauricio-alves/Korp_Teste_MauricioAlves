using Microsoft.EntityFrameworkCore;
using BillingService.Data;
using BillingService.Models;

namespace BillingService.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _context;

    public InvoiceRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .AsNoTracking()
            .OrderByDescending(i => i.Number)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<int> GetNextNumberAsync()
    {
        var maxNumber = await _context.Invoices
            .AsNoTracking()
            .Select(i => (int?)i.Number)
            .MaxAsync();
        return (maxNumber ?? 0) + 1;
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<InvoiceItem> AddItemAsync(InvoiceItem item)
    {
        _context.InvoiceItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task RemoveItemAsync(InvoiceItem item)
    {
        _context.InvoiceItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}
