using Orchware.Backoffice.Domain.Enums;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Inventory;

public class Product : Entity<int>
{
	public string Name { get; set; } = string.Empty;
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public UnitsOfMeasure Units { get; set; }
	public string Image { get; set; } = string.Empty;
	public int ShelfId { get; set; }
	public Shelf Shelf { get; set; } = new();
}
