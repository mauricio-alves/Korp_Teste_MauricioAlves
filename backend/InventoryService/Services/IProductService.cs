using InventoryService.DTOs;

namespace InventoryService.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto);
    Task DeleteAsync(Guid id);
    Task DebitBalanceAsync(Guid id, int quantity);
    Task CreditBalanceAsync(Guid id, int quantity);
}
