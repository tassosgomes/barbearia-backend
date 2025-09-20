using System;
using Barbearia.Application.Common.Persistence;
using Barbearia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Barbearia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlDatabase");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Connection string 'SqlDatabase' was not found.");
        services.AddDbContext<BarbeariaDbContext>(options => options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(BarbeariaDbContext).Assembly.FullName)));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
