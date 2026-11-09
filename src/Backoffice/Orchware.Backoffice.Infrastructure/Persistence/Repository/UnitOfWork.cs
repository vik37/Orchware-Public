using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Primitives;


namespace Orchware.Backoffice.Infrastructure.Persistence.Repository;

public class UnitOfWork : IUnitOfWork
{
	private readonly OrchwareBackofficeDbContext _context;

	public UnitOfWork(OrchwareBackofficeDbContext context)
	{
		_context = context;
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		=> await _context.SaveChangesAsync(cancellationToken);

	public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
	{
		var strategy = _context.Database.CreateExecutionStrategy();

		await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				await action();
				await _context.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				throw;
			}
		});
	}

	public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
	{
		var strategy = _context.Database.CreateExecutionStrategy();

		return await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				var result = await action();
				await _context.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
				return result;
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				throw;
			}
		});
	}
}
