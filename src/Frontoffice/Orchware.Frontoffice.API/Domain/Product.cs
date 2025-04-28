
namespace Orchware.Frontoffice.API.Domain;

public class Product : BaseNamed
{
	public SeasonalFruits SeasonalFruits { get; set; }
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public UnitsOfMeasure Units { get; set; }
	public string Image { get; set; } = string.Empty;
}

public enum SeasonalFruits
{
	Winter = 1,
	Spring,
	Summer
}

public enum UnitsOfMeasure
{
	Grams = 1,
	Kilograms = 2,
	Tones = 3
}
