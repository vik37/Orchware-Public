using Orchware.Backoffice.Domain.Enums;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Inventory;

public sealed class Shelf : AggregateRoot<int>
{
	public string Code { get; set; } = string.Empty;
	public SeasonalFruits SeasonalFruits { get; set; }
	public IList<Product> Products { get; set; } = new List<Product>();
}
