namespace ApiGateway.Models;

public record CreateProductDto(
    string Code,
    string Description,
    decimal InitialBalance
);

public record UpdateProductDto(
    string Code,
    string Description
);
