using Microsoft.AspNetCore.Mvc;
using ApiGateway.Providers;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryProvider _inventory;

    public ProductsController(IInventoryProvider inventory)
    {
        _inventory = inventory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _inventory.GetAsync("/api/products");
        return Content(result, "application/json");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _inventory.GetAsync($"/api/products/{id}");
        return Content(result, "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object body)
    {
        var result = await _inventory.PostAsync("/api/products", body);
        return new ContentResult
        {
            Content = result,
            ContentType = "application/json",
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] object body)
    {
        var result = await _inventory.PutAsync($"/api/products/{id}", body);
        return Content(result, "application/json");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _inventory.DeleteAsync($"/api/products/{id}");
        return NoContent();
    }
}
