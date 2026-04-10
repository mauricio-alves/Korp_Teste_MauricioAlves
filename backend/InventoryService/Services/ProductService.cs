using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(ToDto);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id '{id}' not found.");
        return ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var existing = await _repository.GetByCodeAsync(dto.Code);
        if (existing is not null)
            throw new InvalidOperationException($"A product with code '{dto.Code}' already exists.");

        var product = new Product
        {
            Code = dto.Code,
            Description = dto.Description,
            Balance = dto.Balance
        };

        var created = await _repository.CreateAsync(product);
        return ToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id '{id}' not found.");

        var codeConflict = await _repository.GetByCodeAsync(dto.Code);
        if (codeConflict is not null && codeConflict.Id != id)
            throw new InvalidOperationException($"A product with code '{dto.Code}' already exists.");

        product.Code = dto.Code;
        product.Description = dto.Description;
        product.Balance = dto.Balance;

        var updated = await _repository.UpdateAsync(product);
        return ToDto(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id '{id}' not found.");
        await _repository.DeleteAsync(product);
    }

    public async Task DebitBalanceAsync(Guid id, int quantity)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id '{id}' not found.");

        if (product.Balance < quantity)
            throw new InvalidOperationException($"Insufficient balance for product '{product.Code}'. Available: {product.Balance}, Requested: {quantity}.");

        product.Balance -= quantity;
        await _repository.UpdateAsync(product);
    }

    public async Task CreditBalanceAsync(Guid id, int quantity)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id '{id}' not found.");

        product.Balance += quantity;
        await _repository.UpdateAsync(product);
    }

    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Code, p.Description, p.Balance, p.CreatedAt, p.UpdatedAt);
}
