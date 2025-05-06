using DbQueryBuilder.SqlPremmissionProvider;
using FluentValidation;
using Orchware.Frontoffice.API.Common.Pagginations;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public class FilteredProductValidator : AbstractValidator<FilteredProductsCommand>
{
	public FilteredProductValidator(SqlExpressionValidator sqlValidator)
	{
		RuleFor(x => x.Filter).NotNull();

		RuleFor(x => x.Filter.PageIndex)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater then 0.");

		RuleFor(x => x.Filter.PageSize)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

		RuleFor(x => x.Filter.OrderDirection)
			.Must(dir => dir == "ASC" || dir == "DESC")
			.When(x => !string.IsNullOrWhiteSpace(x.Filter.OrderDirection))
			.WithMessage("{PropertyName} must be either 'ASC' or 'DESC'.");

		RuleFor(x => x.Filter.OrderBy)
			.Must((command, orderBy) =>
				string.IsNullOrWhiteSpace(orderBy) || sqlValidator.IsSafeExpression("Product", $"{orderBy} = 1"))
			.WithMessage("{PropertyName} contains an invalid or unsafe field.");

		RuleFor(x => x.Filter.Search)
			.MaximumLength(100).WithMessage("Search cannot exceed 100 characters.");

		RuleFor(x => x.Filter.OrderBy)
			.Must((command, orderBy) =>
				string.IsNullOrWhiteSpace(orderBy) || sqlValidator.IsSafeExpression("Product", $"{orderBy} = 1"))
			.WithMessage("{PropertyName} contains an invalid or unsafe field.");

		When(x => x.Filter.MultyFilter != null && x.Filter.MultyFilter.Any(), () =>
		{
			RuleForEach(x => x.Filter.MultyFilter)
				.SetValidator(new FilterKeyValueValidator(sqlValidator));
		});
	}
}

public class FilterKeyValueValidator : AbstractValidator<FilterKeyValue>
{
	private readonly SqlExpressionValidator _sqlValidator;

	public FilterKeyValueValidator(SqlExpressionValidator sqlValidator)
	{
		_sqlValidator = sqlValidator;

		RuleFor(x => x.Key)
			.NotEmpty().WithMessage("Key is required.");

		RuleFor(x => x.Values)
			.NotNull().WithMessage("Values list is required.")
			.Must(v => v.Count > 0).WithMessage("At least one value is required for the key.");

		RuleFor(x => x)
			.Must(kv =>
			{
				return _sqlValidator.IsSafeExpression("Product", $"{kv.Key} {kv.Condition} something");
			})
			.WithMessage(kv => $"Invalid or unsafe filter key: '{kv.Key}'.");
	}
}