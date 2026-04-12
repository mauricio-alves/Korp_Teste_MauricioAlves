using ApiGateway.Providers;
using ApiGateway.DTOs;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IBillingProvider _billing;

    public InvoicesController(IBillingProvider billing)
    {
        _billing = billing;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _billing.GetAsync("/api/invoices");
        return Content(result, "application/json");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _billing.GetAsync($"/api/invoices/{id}");
        return Content(result, "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto body)
    {
        var result = await _billing.PostAsync("/api/invoices", body);
        return Content(result, "application/json");
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddInvoiceItemDto body)
    {
        var result = await _billing.PostAsync($"/api/invoices/{id}/items", body);
        return Content(result, "application/json");
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
    {
        await _billing.DeleteAsync($"/api/invoices/{id}/items/{itemId}");
        return NoContent();
    }

    [HttpPost("{id:guid}/print")]
    public async Task<IActionResult> Print(Guid id)
    {
        var result = await _billing.PostAsync($"/api/invoices/{id}/print");
        return Content(result, "application/json");
    }
}
