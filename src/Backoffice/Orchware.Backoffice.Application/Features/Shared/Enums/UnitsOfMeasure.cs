using System.ComponentModel.DataAnnotations;

namespace Orchware.Backoffice.Application.Features.Shared.Enums;

public enum UnitsOfMeasure
{
	[Display(Name = "g")]
	Grams = 1,
	[Display(Name = "kg")]
	Kilograms = 2,
	[Display(Name = "t")]
	Tones = 3
}
