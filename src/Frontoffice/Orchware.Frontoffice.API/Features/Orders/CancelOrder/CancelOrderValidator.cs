using FluentValidation;

namespace Orchware.Frontoffice.API.Features.Orders.CancelOrder;

public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
	public CancelOrderValidator()
	{
		RuleFor(x => x.OrderId).NotEmpty();

		RuleFor(x => x.Reason).NotEmpty().WithMessage("Must be a reason why you cancle the order.")
			.MinimumLength(5).WithMessage("Text must be grater then 5 characters!");
	}
}
