namespace BillingService.DTOs;

public record InvoiceItemDto(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductDescription,
    int Quantity
);

public record InvoiceDto(
    Guid Id,
    int Number,
    string Status,
    DateTime CreatedAt,
    DateTime? ClosedAt,
    List<InvoiceItemDto> Items
);

public record CreateInvoiceDto();

public record AddInvoiceItemDto(
    Guid ProductId,
    string ProductCode,
    string ProductDescription,
    int Quantity
);
