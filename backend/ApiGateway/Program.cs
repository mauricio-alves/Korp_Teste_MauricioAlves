using ApiGateway.Configuration;
using ApiGateway.Middleware;
using ApiGateway.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Korp API Gateway", Version = "v1" });
});

var inventoryUrl = builder.Configuration["Services:InventoryServiceUrl"]
    ?? throw new InvalidOperationException("InventoryServiceUrl not configured.");

var billingUrl = builder.Configuration["Services:BillingServiceUrl"]
    ?? throw new InvalidOperationException("BillingServiceUrl not configured.");

builder.Services.AddHttpClient("InventoryClient", client =>
{
    client.BaseAddress = new Uri(inventoryUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("BillingClient", client =>
{
    client.BaseAddress = new Uri(billingUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("GeminiClient");

builder.Services.AddScoped<IInventoryProvider, InventoryProvider>();
builder.Services.AddScoped<IBillingProvider, BillingProvider>();
builder.Services.AddScoped<IGeminiProvider, GeminiProvider>();

builder.Services.AddApiRateLimiting();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors();
app.MapControllers().RequireRateLimiting("fixed");
app.Run();

public partial class Program { }
