using FluentValidation;

namespace Orchware.Backoffice.Application.Features.Orders.Commands.FilterHistoryOrder;

public class FilterHistoryOrderValidator : AbstractValidator<FilterHistoryOrderCommand>
{
	private static readonly string[] AllowedSortColumns = new[]
	{
		"OrderedDate", "OrderNumber", "Status", "Buyer", "Company", "TotalAmount"
	};

	private static readonly string[] AllowedFilterKeys = new[]
	{
		"Status", "from", "to", "Buyer", "Company"
	};

	private static readonly string[] AllowedOrderDirections = new[]
	{
		"ASC", "DESC", "ASCENDING", "DESCENDING"
	};

	public FilterHistoryOrderValidator()
	{
		RuleFor(x => x.Filter)
			.NotNull().WithMessage("Filter object cannot be null.");

		RuleFor(x => x.Filter.PageIndex)
			.GreaterThan(0).WithMessage("PageIndex must be greater than 0.");

		RuleFor(x => x.Filter.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be greater than 0.");

		RuleFor(x => x.Filter.OrderBy)
			.Must(orderBy => string.IsNullOrEmpty(orderBy) || AllowedSortColumns.Contains(orderBy))
			.WithMessage($"Invalid OrderBy column. Allowed values: {string.Join(", ", AllowedSortColumns)}.");

		RuleFor(x => x.Filter.OrderDirection)
			.Must(direction => string.IsNullOrEmpty(direction) ||
				AllowedOrderDirections.Contains(direction.Trim().ToUpper()))
			.WithMessage("OrderDirection must be either ASC, DESC, Ascending, or Descending.");

		RuleForEach(x => x.Filter.MultyFilter)
			.ChildRules(filter =>
			{
				filter.RuleFor(f => f.Key)
					.Must(key => !string.IsNullOrEmpty(key) && AllowedFilterKeys.Any(allowed => key.Contains(allowed)))
					.WithMessage($"Invalid filter key. Allowed keys must contain: {string.Join(", ", AllowedFilterKeys)}.");

				filter.RuleFor(f => f.Values)
					.Must((f, values) =>
					{
						if (f.Key != "from" && f.Key != "to") return true;
						if (values == null || !values.Any()) return false;

						foreach (var val in values)
						{
							if (!DateTime.TryParse(val, out var parsedDate)) return false;
							if (parsedDate > DateTime.Now) return false;
						}
						return true;
					})
					.WithMessage("The 'from' and 'to' filters must contain valid datetime values and cannot be in the future.");
			});

		RuleFor(x => x.Filter.MultyFilter)
			.Must(filters =>
			{
				if (filters == null) return true;

				var fromFilter = filters.FirstOrDefault(f => f.Key?.Equals("from", StringComparison.OrdinalIgnoreCase) == true);
				var toFilter = filters.FirstOrDefault(f => f.Key?.Equals("to", StringComparison.OrdinalIgnoreCase) == true);

				if (fromFilter?.Values?.FirstOrDefault() != null && toFilter?.Values?.FirstOrDefault() != null)
				{
					if (DateTime.TryParse(fromFilter.Values.First(), out var fromDate) &&
						DateTime.TryParse(toFilter.Values.First(), out var toDate))
					{
						return toDate >= fromDate; // 'to' не смее да биде порано од 'from'
					}
				}
				return true;
			})
			.WithMessage("The 'to' date cannot be earlier than the 'from' date.");
	}
}
