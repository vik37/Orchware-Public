using FluentValidation;

namespace Orchware.Frontoffice.API.Features.Orders.FilterHistoryOrder;

public class FilterHistoryOrderValidator : AbstractValidator<FilterHistoryOrderCommand>
{
	public FilterHistoryOrderValidator()
	{
		RuleFor(x => x.Filter.PageIndex)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater then 0.");

		RuleFor(x => x.Filter.PageSize)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

		RuleFor(x => x.Filter.OrderStatus)
			.Must(status => string.IsNullOrWhiteSpace(status) || status == "Cancelled" || status == "Approved")
			.WithMessage("OrderStatus must be either 'Cancelled' or 'Approved'.");

		RuleFor(x => x.Filter.From)
			.LessThanOrEqualTo(DateTime.Now)
			.When(x => x.Filter.From.HasValue)
			.WithMessage("From date cannot be in the future.");

		RuleFor(x => x.Filter.To)
			.LessThanOrEqualTo(DateTime.Now)
			.When(x => x.Filter.To.HasValue)
			.WithMessage("To date cannot be in the future.");

		RuleFor(x => x)
			.Must(x => !x.Filter.From.HasValue || !x.Filter.To.HasValue || x.Filter.From.Value <= x.Filter.To.Value)
			.WithMessage("The 'From' date cannot be greater than the 'To' date.");
	}
}
