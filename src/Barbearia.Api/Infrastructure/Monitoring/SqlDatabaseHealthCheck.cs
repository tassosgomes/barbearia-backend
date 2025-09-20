using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Barbearia.Api.Infrastructure.Monitoring;

public sealed class SqlDatabaseHealthCheck : IHealthCheck
{
    private readonly string connectionString;

    public SqlDatabaseHealthCheck(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("SqlDatabase") ?? throw new InvalidOperationException("Connection string 'SqlDatabase' is required for health checks.");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Unable to connect to SQL Server.", exception);
        }
    }
}
