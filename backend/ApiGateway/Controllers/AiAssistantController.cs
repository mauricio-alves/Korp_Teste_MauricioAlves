using Microsoft.AspNetCore.Mvc;
using ApiGateway.Providers;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/ai")]
public class AiAssistantController : ControllerBase
{
    private readonly IGeminiProvider _gemini;

    public AiAssistantController(IGeminiProvider gemini)
    {
        _gemini = gemini;
    }

    [HttpPost("suggest-products")]
    [ProducesResponseType(typeof(SuggestProductsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SuggestProducts([FromBody] SuggestProductsRequest request)
    {
        var suggestion = await _gemini.SuggestProductsAsync(request.Context, request.AvailableProducts);
        return Ok(new SuggestProductsResponse(suggestion));
    }
}

public record SuggestProductsRequest(string Context, string AvailableProducts);
public record SuggestProductsResponse(string Suggestion);
