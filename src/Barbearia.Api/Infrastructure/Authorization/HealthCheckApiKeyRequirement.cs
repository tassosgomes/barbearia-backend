using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Api.Infrastructure.Authorization;

public sealed record HealthCheckApiKeyRequirement : IAuthorizationRequirement;
