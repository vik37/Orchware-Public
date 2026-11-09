namespace Orchware.Frontoffice.API.Features.Orders.Shared.Models;

public class ProductDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string SeasonalFruits { get; set; } = string.Empty;
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public string Units { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}
