namespace Barbearia.Infrastructure.DependencyInjection;

using Barbearia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BarbeariaDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'BarbeariaDatabase' is not configured");
        }
        services.AddDbContext<BarbeariaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        return services;
    }
}
