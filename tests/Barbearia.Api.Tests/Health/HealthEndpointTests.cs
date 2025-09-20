namespace Barbearia.Api.Tests.Health;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["HealthChecks:ApiKey"] = "test-key",
                    ["ConnectionStrings:BarbeariaDatabase"] = "Server=localhost,1433;Database=Barbearia;User Id=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True"
                };
                configurationBuilder.AddInMemoryCollection(overrides);
            });
        });
    }

    [Fact]
    public async Task GetHealth_WithValidApiKey_ReturnsSuccessStatus()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var request = new HttpRequestMessage(HttpMethod.Get, "/healthz");
        request.Headers.Add("X-Health-Api-Key", "test-key");
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_WithMissingApiKey_ReturnsUnauthorizedStatus()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
