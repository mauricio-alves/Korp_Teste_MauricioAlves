namespace InventoryService.DTOs;

public record ProductDto(
    Guid Id,
    string Code,
    string Description,
    int Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateProductDto(string Code, string Description, int Balance);

public record UpdateProductDto(string Code, string Description, int Balance);

public record UpdateBalanceDto(int Quantity);
