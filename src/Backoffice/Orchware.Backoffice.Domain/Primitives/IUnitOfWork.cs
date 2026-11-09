using Microsoft.EntityFrameworkCore.Storage;

namespace Orchware.Backoffice.Domain.Primitives;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
	Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
