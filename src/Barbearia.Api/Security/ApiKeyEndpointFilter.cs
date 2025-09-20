namespace Barbearia.Api.Security;

using System.Security.Cryptography;
using System.Text;

internal sealed class ApiKeyEndpointFilter : IEndpointFilter
{
    private const string ApiKeyHeaderName = "X-Health-Api-Key";
    private readonly byte[] configuredKey;

    internal ApiKeyEndpointFilter(string? apiKey)
    {
        configuredKey = string.IsNullOrWhiteSpace(apiKey) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(apiKey);
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (configuredKey.Length == 0)
        {
            return Results.Problem("Health check API key is not configured", statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKeyValues))
        {
            return Results.Unauthorized();
        }
        var providedKey = providedKeyValues.ToString();
        if (string.IsNullOrEmpty(providedKey))
        {
            return Results.Unauthorized();
        }
        var providedKeyBytes = Encoding.UTF8.GetBytes(providedKey);
        if (providedKeyBytes.Length != configuredKey.Length)
        {
            return Results.Unauthorized();
        }
        if (!CryptographicOperations.FixedTimeEquals(providedKeyBytes, configuredKey))
        {
            return Results.Unauthorized();
        }
        return await next(context);
    }
}
