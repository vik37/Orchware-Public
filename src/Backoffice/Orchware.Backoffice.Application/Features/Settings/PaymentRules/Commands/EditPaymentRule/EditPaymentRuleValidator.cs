using FluentValidation;

namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.Commands.EditPaymentRule;

public class EditPaymentRuleValidator : AbstractValidator<EditPaymentRuleCommand>
{
	private const int MaxInstallments = 36;
	private const int MaxGracePeriodDays = 12;

	private static bool HaveMaximumTwoDecimalPlaces(decimal value) => decimal.Round(value, 2) == value;

	public EditPaymentRuleValidator()
	{
		RuleFor(x => x.Model.Id)
		   .GreaterThan(0);

		RuleFor(x => x.Model.MaxAllowedInstallments)
			.InclusiveBetween(1, MaxInstallments)
			.WithMessage($"Max allowed installments must be between 1 and {MaxInstallments}.");

		RuleFor(x => x.Model.GracePeriod)
			.InclusiveBetween(1, MaxGracePeriodDays)
			.WithMessage($"Grace period must be between 1 and {MaxGracePeriodDays} days.");

		When(x => !x.Model.AllowDiscountModification, () =>
		{
			RuleFor(x => x.Model.DiscountDescription)
				.Null()
				.WithMessage("Discount description must be null when discount modification is disabled.");
		});

		When(x => x.Model.AllowDiscountModification, () =>
		{
			RuleFor(x => x.Model.DiscountDescription)
				.NotNull()
				.WithMessage("Discount description is required when discount modification is enabled.");

			When(x => x.Model.DiscountDescription is not null, () =>
			{
				RuleFor(x => x.Model.DiscountDescription!.MinimumOrderAmountForDiscount)
					.GreaterThan(0)
					.WithMessage("Minimum order amount for discount must be greater than 0.");

				RuleFor(x => x.Model.DiscountDescription!.OrderAmountDiscountPercentage)
					.InclusiveBetween(0, 100)
					.WithMessage("Order amount discount percentage must be between 0 and 100.")
					.Must(HaveMaximumTwoDecimalPlaces)
					.WithMessage("Order amount discount percentage can have at most 2 decimal places.");

				RuleFor(x => x.Model.DiscountDescription!.PayImmediatelyDiscountPercentage)
					.InclusiveBetween(0, 100)
					.WithMessage("Pay immediately discount percentage must be between 0 and 100.")
					.Must(HaveMaximumTwoDecimalPlaces)
					.WithMessage("Pay immediately discount percentage can have at most 2 decimal places.");
			});
		});
	}
}
