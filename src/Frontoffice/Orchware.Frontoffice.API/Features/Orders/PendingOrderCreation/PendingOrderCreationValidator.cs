using FluentValidation;
using Orchware.Frontoffice.API.Features.Products.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Orders.PendingOrderCreation;

public class PendingOrderCreationValidator : AbstractValidator<PendingOrderCreationCommand>
{
	public PendingOrderCreationValidator()
	{
		RuleForEach(x => x.Details).SetValidator(new OrderDetailsDtoValidator());
	}
}

public class OrderDetailsDtoValidator : AbstractValidator<OrderDetailsDto>
{
	public OrderDetailsDtoValidator()
	{
		RuleFor(x => x.Product).NotNull().WithMessage("Missing Product.");

		RuleFor(x => x)
				.Must(x => x.Product.AvailableQuantity >= x.Product.MinQuantity)
				.WithMessage(x => $"This product cannot be ordered because the available stock ({x.Product.AvailableQuantity} {x.Product.Units}) is below the minimum order quantity ({x.Product.MinQuantity} {x.Product.Units}).");

		RuleFor(x => x)
				.Must(x =>
				{
					var quantityInKg = UnitConverter.Convert(x.Quantity,x.UnitOfMeasure,x.Product.Units);
					return quantityInKg >= x.Product.MinQuantity;
				})
				.WithMessage(x => $"The quantity must be at least {x.Product.MinQuantity} {x.Product.Units}");

		RuleFor(x => x)
				.Must(x =>
				{
					var quantityInKg = UnitConverter.Convert(x.Quantity, x.UnitOfMeasure, x.Product.Units);
					return quantityInKg <= (decimal)x.Product.AvailableQuantity;
				})
				.WithMessage(x => $"There is not enough stock. Maximum: {x.Product.AvailableQuantity} {x.Product.Units}");

		RuleFor(x => x.UnitOfMeasure)
			.Must(BeValidUnit)
			.WithMessage("Invalid unit of measure. Allowed are: g, kg, t");
	}

	private bool BeValidUnit(string unit)
	{
		var validUnits = Enum.GetValues(typeof(UnitsOfMeasure))
							 .Cast<UnitsOfMeasure>()
							 .Select(u => u.GetDisplayNameToString());

		return validUnits.Any(x => x == unit);
	}
}

