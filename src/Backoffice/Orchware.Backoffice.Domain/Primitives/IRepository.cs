namespace Orchware.Backoffice.Domain.Primitives;

public interface IRepository<TEntity, TId>
	where TEntity : class, IAggregateRoot
{
	Task<IList<TEntity>> GetAllAsync();
	Task<TEntity?> GetByIdAsync(TId id);
	Task AddAsync(TEntity entity);
	Task UpdateAsync(TEntity entity);
	Task DeleteAsync(TId id);

	public IUnitOfWork UnitOfWork { get; }
}