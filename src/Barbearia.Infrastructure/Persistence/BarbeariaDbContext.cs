namespace Barbearia.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public sealed class BarbeariaDbContext : DbContext
{
    public BarbeariaDbContext(DbContextOptions<BarbeariaDbContext> options) : base(options)
    {
    }
}
