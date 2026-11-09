using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Infrastructure.Persistence.Repository;

public class ShelfRepository : IShelfRepository
{
	private readonly OrchwareBackofficeDbContext _context;

	public IUnitOfWork UnitOfWork { get; }

	public ShelfRepository(OrchwareBackofficeDbContext context, IUnitOfWork unitOfWork)
	{
		_context = context;
		UnitOfWork = unitOfWork;
	}

	public Task<IList<Shelf>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public async Task<Shelf?> GetByIdAsync(int id)
	{
		return await _context.Shelf.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<Dictionary<string, (string shelfCode, int shelfProductId)>>GetShelfInfoForProductsAsync(
		IEnumerable<string> productNames, CancellationToken ct = default)
	{
		return await _context.Shelf
			.Include(s => s.Products)
			.SelectMany(s => s.Products, (s, p) => new
			{
				ProductName = p.Name,
				ShelfCode = s.Code,
				ShelfProductId = p.Id
			})
			.Where(x => productNames.Contains(x.ProductName))
			.ToDictionaryAsync(
				x => x.ProductName,
				x => (x.ShelfCode, x.ShelfProductId),
				ct);
	}

	public async Task AddAsync(Shelf entity)
	{
		await _context.AddAsync(entity);
	}

	public async Task DeleteAsync(int id)
	{
		var shelf = await _context.Shelf.FirstOrDefaultAsync(x => x.Id == id);

		if(shelf is not null)
		{
			_context.Shelf.Remove(shelf);
		}
	}

	public Task UpdateAsync(Shelf entity)
	{
		_context.Shelf.Update(entity);
		return Task.CompletedTask;
	}

	public async Task<Shelf?> GetShelfByCode(string shelfCode, CancellationToken ct = default)
	{
		return await _context.Shelf.Include(x => x.Products).FirstOrDefaultAsync(x => x.Code == shelfCode, ct);
	}
}
