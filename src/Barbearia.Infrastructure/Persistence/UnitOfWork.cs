using System.Threading;
using System.Threading.Tasks;
using Barbearia.Application.Common.Persistence;

namespace Barbearia.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly BarbeariaDbContext context;

    public UnitOfWork(BarbeariaDbContext context)
    {
        this.context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
