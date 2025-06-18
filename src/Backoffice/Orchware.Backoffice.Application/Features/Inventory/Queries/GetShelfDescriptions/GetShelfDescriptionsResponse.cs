using Orchware.Backoffice.Application.Features.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetShelfDescriptions;

public class GetShelfDescriptionsResponse
{
	private int _totalProducts = 0;
	public int TotalProducts { get { return _totalProducts; } }

	public int TotalShelves { get => Shelves != null ? Shelves.Count() : 0; }

	public IEnumerable<ShelfDto> Shelves { get; set; } = new List<ShelfDto>();

	public GetShelfDescriptionsResponse(int totalProducts)
	{
		_totalProducts = totalProducts;
	}
}

public class ShelfDto
{
	public int Id { get; }
	public string Code { get; } = string.Empty;

	private SeasonalFruits SeasonalFruits { get; }
	public string SeasonalFruit => SeasonalFruits.GetDisplayNameToString();

	public IReadOnlyCollection<ProductDto> Products => _products;
	private readonly List<ProductDto> _products = new();

	public void AddProducts(ProductDto product)
	{
		_products.Add(product);
	}
}

public class ProductDto
{
	public string Name { get; } = string.Empty;
	public float AvailableQuantity { get; }

	private UnitsOfMeasure Units { get; }

	public string Unit => Units.GetDisplayNameToString();
}
