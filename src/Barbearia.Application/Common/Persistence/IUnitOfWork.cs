using System.Threading;
using System.Threading.Tasks;

namespace Barbearia.Application.Common.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
