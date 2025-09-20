using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Barbearia.Api.Infrastructure.Authorization;

public sealed class HealthCheckApiKeyHandler : AuthorizationHandler<HealthCheckApiKeyRequirement>
{
    private const string HeaderName = "X-Health-Api-Key";
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;

    public HealthCheckApiKeyHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HealthCheckApiKeyRequirement requirement)
    {
        var configuredKey = configuration["Monitoring:HealthCheckKey"];
        if (string.IsNullOrWhiteSpace(configuredKey)) return Task.CompletedTask;
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return Task.CompletedTask;
        if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)) return Task.CompletedTask;
        var providedKeyValue = providedKey.ToString();
        if (string.IsNullOrWhiteSpace(providedKeyValue)) return Task.CompletedTask;
        var configuredKeyBytes = Encoding.UTF8.GetBytes(configuredKey);
        var providedKeyBytes = Encoding.UTF8.GetBytes(providedKeyValue);
        if (configuredKeyBytes.Length != providedKeyBytes.Length) return Task.CompletedTask;
        if (!CryptographicOperations.FixedTimeEquals(configuredKeyBytes, providedKeyBytes)) return Task.CompletedTask;
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
