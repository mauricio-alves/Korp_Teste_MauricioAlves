namespace ApiGateway.DTOs;

public record CreateInvoiceDto(List<CreateInvoiceItemDto> Items);
public record CreateInvoiceItemDto(Guid ProductId, string ProductCode, string ProductDescription, int Quantity);
public record AddInvoiceItemDto(Guid ProductId, string ProductCode, string ProductDescription, int Quantity);
