using System;
using Barbearia.Api.Infrastructure.Authorization;
using Barbearia.Api.Infrastructure.Monitoring;
using Barbearia.Application;
using Barbearia.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("SqlDatabase"))) throw new InvalidOperationException("Connection string 'SqlDatabase' is required.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, HealthCheckApiKeyHandler>();
builder.Services.AddHealthChecks().AddCheck<SqlDatabaseHealthCheck>("sql-database", tags: new[] { "database" });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(HealthCheckPolicies.HealthCheck, policy => policy.AddRequirements(new HealthCheckApiKeyRequirement()));
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["Telemetry:ServiceName"] ?? "barbearia-api"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddConsoleExporter();
    });

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.AddConsoleExporter();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (!app.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER", false)) app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = HealthCheckResponseWriter.WriteAsync }).RequireAuthorization(HealthCheckPolicies.HealthCheck);
app.Run();
