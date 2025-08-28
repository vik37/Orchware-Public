using FluentValidation;
using Orchware.Frontoffice.API.Common.CustomExtensions;

namespace Orchware.Frontoffice.API.Features.Company.EditUserJobTitle;

public class EditUserCompanyJobTitleValidator : AbstractValidator<EditUserCompanyJobTitleCommand>
{
	public EditUserCompanyJobTitleValidator()
	{
		RuleFor(x => x.CompanyId).NotEmpty().GreaterThan(0).WithMessage("Invalid Company Identifier");

		RuleFor(x => x.JobTitle)
			.NotEmpty().WithMessage("Job Title is required.")
			.MaximumLength(850).WithMessage("Job Title cannot exceed 850 characters.")
			.MustNotHaveRepeatingCharacters();
	}
}
