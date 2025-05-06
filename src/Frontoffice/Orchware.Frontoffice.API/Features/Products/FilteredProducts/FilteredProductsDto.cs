
namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public class FilteredProductsDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string SeasonalFruits { get; private set; } = string.Empty;
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public string Units { get; private set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}
