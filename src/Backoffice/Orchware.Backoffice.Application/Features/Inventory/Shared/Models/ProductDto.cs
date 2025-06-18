using Orchware.Backoffice.Application.Features.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Inventory.Shared.Models;

public class ProductDto
{
	public int Id { get; }
	public string Name { get; } = string.Empty;
	public float AvailableQuantity { get; }
	public decimal Price { get; }
	public int MinQuantity { get; }

	private UnitsOfMeasure Units { get; }

	public string Unit => Units.GetDisplayNameToString();

	public string Image { get; set; } = string.Empty;
}