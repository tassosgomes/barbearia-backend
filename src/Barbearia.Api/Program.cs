<<<<<<< HEAD
using Barbearia.Api.Health;
using Barbearia.Api.Security;
using Barbearia.Application.DependencyInjection;
using Barbearia.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext().WriteTo.Console();
});

builder.Services.AddHealthChecks();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddOpenTelemetry().ConfigureResource(resource => resource.AddService("Barbearia.Api")).WithMetrics(metrics =>
{
    metrics.AddAspNetCoreInstrumentation();
    metrics.AddHttpClientInstrumentation();
    metrics.AddRuntimeInstrumentation();
    metrics.AddOtlpExporter();
}).WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddOtlpExporter();
});

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapGet("/healthz", async (HealthCheckService healthCheckService) =>
{
    var report = await healthCheckService.CheckHealthAsync();
    var components = report.Entries.Select(entry => new HealthCheckComponent(entry.Key, entry.Value.Status.ToString(), entry.Value.Description ?? string.Empty)).ToArray();
    var response = new HealthCheckResponse(report.Status.ToString(), components);
    return report.Status == HealthStatus.Healthy ? Results.Ok(response) : Results.Problem("Health check failed", statusCode: StatusCodes.Status503ServiceUnavailable, extensions: new Dictionary<string, object?> { ["components"] = response.Components });
}).AddEndpointFilter(new ApiKeyEndpointFilter(builder.Configuration["HealthChecks:ApiKey"]))
.WithName("HealthCheck");

app.Run();

public partial class Program;
=======
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
>>>>>>> 2e21ea162011e45977563e194d64e4e0679d8483
