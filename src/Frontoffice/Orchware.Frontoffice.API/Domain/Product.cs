using System.ComponentModel.DataAnnotations;

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
	[Display(Name = "Winter")]
	Winter = 1,
	[Display(Name = "Spring")]
	Spring,
	[Display(Name = "Summer")]
	Summer
}

public enum UnitsOfMeasure
{
	[Display(Name = "g")]
	Grams = 1,
	[Display(Name = "kg")]
	Kilograms = 2,
	[Display(Name = "t")]
	Tones = 3
}
