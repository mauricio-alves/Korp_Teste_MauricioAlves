using System.Net;
using System.Text.Json;

namespace ApiGateway.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gateway error: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            HttpRequestException httpEx when httpEx.StatusCode.HasValue => 
                (httpEx.StatusCode.Value, httpEx.StatusCode.Value.ToString()),
            HttpRequestException => (HttpStatusCode.ServiceUnavailable, "Service Unavailable"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Bad Request"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title,
            status = (int)status,
            detail = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
