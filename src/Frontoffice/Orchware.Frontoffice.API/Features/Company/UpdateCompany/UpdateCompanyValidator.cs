using FluentValidation;
using Orchware.Frontoffice.API.Common.CustomExtensions;

namespace Orchware.Frontoffice.API.Features.Company.UpdateCompany;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
	public UpdateCompanyValidator()
	{
		RuleFor(x => x.CompanyId).NotEmpty().GreaterThan(0).WithMessage("Invalid Company Identifier");

		RuleFor(x => x.CompanyName)
			.NotEmpty().WithMessage("Company Name is required.")
			.MinimumLength(1).WithMessage("Job Title is required.")
			.MaximumLength(550).WithMessage("Company Name cannot exceed 550 characters.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyEmail)
			.NotEmpty().WithMessage("Company Email is required.")
			.EmailAddress().WithMessage("A valid Company Email is required.")
			.MaximumLength(400).WithMessage("Company Email cannot exceed 400 characters.");

		RuleFor(x => x.CompanyAddress)
			.NotEmpty().WithMessage("Address is required.")
			.MinimumLength(1).WithMessage("Job Title is required.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyPhoneNumber)
				.NotEmpty().WithMessage("Phone Number is required.")
				.Matches(@"^\d{8,10}$")
				.WithMessage("Please enter a valid phone number. It should contain 8 to 10 digits only.")
				.MaximumLength(10).WithMessage("Phone Number cannot exceed 10 characters.");

		RuleFor(x => x.CompanyCity)
			.NotEmpty().WithMessage("City is required")
			.MinimumLength(2).WithMessage("City name must be at least 2 characters long.")
			.MaximumLength(100).WithMessage("City cannot exceed 100 characters.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyLocation)
			.NotEmpty().WithMessage("Location is required")
			.MinimumLength(2).WithMessage("City name must be at least 2 characters long.")
			.MaximumLength(250).WithMessage("Location cannot exceed 250 characters.")
			.MustNotHaveRepeatingCharacters();
	}
}
