using Xunit;
using Moq;
using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;
using InventoryService.Services;

namespace InventoryService.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Code = "P001", Description = "Product 1", Balance = 10 },
            new() { Id = Guid.NewGuid(), Code = "P002", Description = "Product 2", Balance = 5 }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Code = "P001", Description = "Product 1", Balance = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        var result = await _sut.GetByIdAsync(id);

        Assert.Equal("P001", result.Code);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_WithUniqueCode_CreatesProduct()
    {
        var dto = new CreateProductDto("P001", "Product 1", 100);
        _repositoryMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync((Product?)null);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await _sut.CreateAsync(dto);

        Assert.Equal("P001", result.Code);
        Assert.Equal(100, result.Balance);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ThrowsInvalidOperationException()
    {
        var dto = new CreateProductDto("P001", "Product 1", 100);
        var existing = new Product { Code = "P001" };
        _repositoryMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task DebitBalanceAsync_WithSufficientBalance_DebitsCorrectly()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Code = "P001", Description = "P", Balance = 50 };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        await _sut.DebitBalanceAsync(id, 20);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Balance == 30)), Times.Once);
    }

    [Fact]
    public async Task DebitBalanceAsync_WithInsufficientBalance_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Code = "P001", Description = "P", Balance = 5 };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DebitBalanceAsync(id, 10));
    }

    [Fact]
    public async Task CreditBalanceAsync_IncreasesBalance()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Code = "P001", Description = "P", Balance = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        await _sut.CreditBalanceAsync(id, 5);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Balance == 15)), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesProduct()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Code = "P001", Description = "P", Balance = 10 };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        await _sut.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(Guid.NewGuid()));
    }
}
