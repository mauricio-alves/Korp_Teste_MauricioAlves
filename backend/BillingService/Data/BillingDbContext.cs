using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.Number).IsUnique();
            entity.Property(i => i.Status).HasConversion<string>();
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasOne(i => i.Invoice)
                  .WithMany(inv => inv.Items)
                  .HasForeignKey(i => i.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
