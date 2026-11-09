using FluentValidation;
using Orchware.Frontoffice.API.Features.Products.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Orders.RequestOrder;

public class RequestOrderValidation : AbstractValidator<RequestOrderCommand>
{
	public RequestOrderValidation()
	{
		RuleForEach(x => x.Details).SetValidator(new OrderDetailsDtoValidator());

		RuleFor(x => x.Payment)
			.NotNull()
			.WithMessage("Payment can not be null.")
			.SetValidator(new PaymentDtoValidator());
	}

	public class PaymentDtoValidator : AbstractValidator<PaymentDto>
	{
		public PaymentDtoValidator()
		{
			RuleFor(x => x.PaymentDate)
				.GreaterThanOrEqualTo(DateTime.Today.AddDays(1))
				.WithMessage("The payment date must be tomorrow at the earliest.");

			When(x => x.WillBePaidInFull == false, () =>
			{
				RuleFor(x => x.NumberOfInstallments)
					.NotNull()
					.WithMessage("The number of installments is mandatory.")
					.GreaterThanOrEqualTo(1)
					.WithMessage("The number of installments must be 1 or greater.");
			}).Otherwise(() => 
			{
				RuleFor(x => x.NumberOfInstallments)
					.Null()
					.WithMessage("The number of installments must be null if the payment is in full.");
			});
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
					var quantityInKg = UnitConverter.Convert(x.Quantity, x.UnitOfMeasure, x.Product.Units);
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
}
