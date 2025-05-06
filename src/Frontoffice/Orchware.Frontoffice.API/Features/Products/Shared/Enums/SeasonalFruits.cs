using System.ComponentModel.DataAnnotations;

namespace Orchware.Frontoffice.API.Features.Products.Shared.Enums;

public enum SeasonalFruits
{
	[Display(Name = "Winter")]
	Winter = 1,
	[Display(Name = "Spring")]
	Spring,
	[Display(Name = "Summer")]
	Summer
}
