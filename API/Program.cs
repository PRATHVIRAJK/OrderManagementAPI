using API.Health;
using API.Middleware;
using Application.Interfaces;
using Application.Services;
using HealthChecks.UI.Client;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


//DI
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
//HealthCheck for database
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
//Check for Traces and Metrics
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("OrdersAPI"))
    .WithMetrics(metrics =>
    {
        metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();
    });
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.AddConsoleExporter().SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OrdersAPI"));
});
//Serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Orders API V1");
    });

    // Redirect root to Swagger UI to avoid unmatched route attempts
    app.MapGet("/", () => Results.Redirect("/swagger"));
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health",new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
