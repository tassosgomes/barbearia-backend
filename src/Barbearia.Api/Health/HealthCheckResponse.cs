namespace Barbearia.Api.Health;

internal sealed record HealthCheckResponse(string Status, IReadOnlyCollection<HealthCheckComponent> Components);

internal sealed record HealthCheckComponent(string Name, string Status, string Description);
