using Xunit;
using Moq;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Providers;
using BillingService.Repositories;
using BillingService.Services;

namespace BillingService.Tests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _repositoryMock;
    private readonly Mock<IInventoryProvider> _inventoryProviderMock;
    private readonly InvoiceService _sut;

    public InvoiceServiceTests()
    {
        _repositoryMock = new Mock<IInvoiceRepository>();
        _inventoryProviderMock = new Mock<IInventoryProvider>();
        _sut = new InvoiceService(_repositoryMock.Object, _inventoryProviderMock.Object);
    }

    [Fact]
    public async Task CreateAsync_AssignsSequentialNumber()
    {
        _repositoryMock.Setup(r => r.GetNextNumberAsync()).ReturnsAsync(5);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => i);

        var dto = new CreateInvoiceDto(new List<AddInvoiceItemDto>());
        var result = await _sut.CreateAsync(dto);

        Assert.Equal(5, result.Number);
        Assert.Equal("Open", result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Invoice?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddItemAsync_ToOpenInvoice_AddsItem()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Number = 1, Status = InvoiceStatus.Open };
        var dto = new AddInvoiceItemDto(Guid.NewGuid(), "P001", "Product 1", 3);

        _repositoryMock.SetupSequence(r => r.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice)
            .ReturnsAsync(invoice);
        _repositoryMock.Setup(r => r.AddItemAsync(It.IsAny<InvoiceItem>()))
            .ReturnsAsync((InvoiceItem item) => item);

        var result = await _sut.AddItemAsync(invoiceId, dto);

        _repositoryMock.Verify(r => r.AddItemAsync(It.IsAny<InvoiceItem>()), Times.Once);
    }

    [Fact]
    public async Task AddItemAsync_ToClosedInvoice_ThrowsInvalidOperationException()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Closed };
        var dto = new AddInvoiceItemDto(Guid.NewGuid(), "P001", "Product 1", 1);

        _repositoryMock.Setup(r => r.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddItemAsync(invoiceId, dto));
    }

    [Fact]
    public async Task PrintAsync_WithOpenInvoice_DebitsAllItemsAndCloses()
    {
        var invoiceId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Number = 1,
            Status = InvoiceStatus.Open,
            Items = [new InvoiceItem { ProductId = productId, Quantity = 5, ProductCode = "P001", ProductDescription = "P" }]
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => i);
        _inventoryProviderMock.Setup(p => p.DebitBalanceAsync(productId, 5)).Returns(Task.CompletedTask);

        var result = await _sut.PrintAsync(invoiceId);

        Assert.Equal("Closed", result.Status);
        _inventoryProviderMock.Verify(p => p.DebitBalanceAsync(productId, 5), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidQuantity_ThrowsArgumentException()
    {
        var dto = new CreateInvoiceDto(
        [
            new AddInvoiceItemDto(Guid.NewGuid(), "P1", "D1", 0)
        ]);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithEmptyProductId_ThrowsArgumentException()
    {
        var dto = new CreateInvoiceDto(
        [
            new AddInvoiceItemDto(Guid.Empty, "P1", "D1", 1)
        ]);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task PrintAsync_WhenDebitFails_ExecutesRollbackAndThrows()
    {
        var invoiceId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Number = 1,
            Status = InvoiceStatus.Open,
            Items =
            [
                new InvoiceItem { ProductId = productId1, Quantity = 2, ProductCode = "P001", ProductDescription = "P1" },
                new InvoiceItem { ProductId = productId2, Quantity = 3, ProductCode = "P002", ProductDescription = "P2" }
            ]
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);
        _inventoryProviderMock.Setup(p => p.DebitBalanceAsync(productId1, 2)).Returns(Task.CompletedTask);
        _inventoryProviderMock.Setup(p => p.DebitBalanceAsync(productId2, 3))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));
        _inventoryProviderMock.Setup(p => p.CreditBalanceAsync(productId1, 2)).Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.PrintAsync(invoiceId));

        _inventoryProviderMock.Verify(p => p.CreditBalanceAsync(productId1, 2), Times.Once);
        _inventoryProviderMock.Verify(p => p.CreditBalanceAsync(productId2, 3), Times.Never);
    }

    [Fact]
    public async Task PrintAsync_WithClosedInvoice_ThrowsInvalidOperationException()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Closed };

        _repositoryMock.Setup(r => r.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.PrintAsync(invoiceId));
    }

    [Fact]
    public async Task PrintAsync_WithEmptyInvoice_ThrowsInvalidOperationException()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Open, Items = [] };

        _repositoryMock.Setup(r => r.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.PrintAsync(invoiceId));
    }
}
