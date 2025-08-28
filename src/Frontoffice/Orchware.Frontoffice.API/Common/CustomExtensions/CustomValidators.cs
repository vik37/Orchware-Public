using FluentValidation;

namespace Orchware.Frontoffice.API.Common.CustomExtensions;

public static class CustomValidators
{
	public static IRuleBuilderOptions<T, string> MustNotHaveRepeatingCharacters<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		return ruleBuilder.Must(value =>
		{
			if (string.IsNullOrWhiteSpace(value))
				return false;

			if (value.All(c => c == value[0]))
				return false;

			if (value.Any(c => char.IsControl(c)))
				return false;

			return true;
		}).WithMessage("{PropertyName} cannot consist of only a single repeated character.");
	}
}
