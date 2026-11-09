using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Inventory;

public interface IShelfRepository : IRepository<Shelf, int>
{
	Task<Dictionary<string, (string shelfCode, int shelfProductId)>>
								GetShelfInfoForProductsAsync(IEnumerable<string> productNames, CancellationToken ct = default);

	Task<Shelf?> GetShelfByCode(string shelfCode, CancellationToken ct = default);
}
