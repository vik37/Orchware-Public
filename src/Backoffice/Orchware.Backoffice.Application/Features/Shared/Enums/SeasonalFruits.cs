using System.ComponentModel.DataAnnotations;

namespace Orchware.Backoffice.Application.Features.Shared.Enums;

public enum SeasonalFruits
{
	[Display(Name = "Winter")]
	Winter = 1,
	[Display(Name = "Spring")]
	Spring,
	[Display(Name = "Summer")]
	Summer
}
