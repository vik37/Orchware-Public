using FluentValidation;
using Orchware.Frontoffice.API.Common.CustomExtensions;

namespace Orchware.Frontoffice.API.Features.Company.AssignUserToCompany;

public class AssignUserToCompanyValidator : AbstractValidator<AssignUserToCompanyCommand>
{
	public AssignUserToCompanyValidator()
	{
		RuleFor(x => x.CompanyId)
			.NotEmpty().WithMessage("Company Id is required.")
			.GreaterThan(0).WithMessage("Company ID of 0 or less then are invalid");

		RuleFor(x => x.JobTitle)
			.NotEmpty().WithMessage("Job Title is required.")
			.MinimumLength(1).WithMessage("Job Title is required.")
			.MaximumLength(850).WithMessage("Job Title cannot exceed 850 characters.")
			.MustNotHaveRepeatingCharacters();
	}
}
