namespace ApiGateway.Models;

public record CreateProductDto(
    string Code,
    string Description,
    int Balance
);

public record UpdateProductDto(
    string Code,
    string Description,
    int Balance
);
