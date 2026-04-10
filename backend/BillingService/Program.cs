using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using BillingService.Data;
using BillingService.Middleware;
using BillingService.Providers;
using BillingService.Repositories;
using BillingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Billing Service API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("BillingDb")
    ?? throw new InvalidOperationException("Connection string 'BillingDb' not found.");

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

var inventoryServiceUrl = builder.Configuration["Services:InventoryServiceUrl"]
    ?? throw new InvalidOperationException("InventoryServiceUrl not configured.");

builder.Services.AddHttpClient("InventoryClient", client =>
{
    client.BaseAddress = new Uri(inventoryServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryProvider, InventoryProvider>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    db.Database.Migrate();
}

app.UseCors();
app.MapControllers();
app.Run();

public partial class Program { }
