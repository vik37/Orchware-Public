using Orchware.Backoffice.Application.Features.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Inventory.Shared.Models;

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

	public void AddRangeOfProducts(IEnumerable<ProductDto> products)
	{
		_products.AddRange(products);
	}
}
