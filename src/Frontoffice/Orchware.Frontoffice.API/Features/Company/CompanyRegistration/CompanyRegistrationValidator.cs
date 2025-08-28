using FluentValidation;
using Orchware.Frontoffice.API.Common.CustomExtensions;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistration;

public class CompanyRegistrationValidator : AbstractValidator<CompanyRegistrationCommand>
{
	public CompanyRegistrationValidator()
	{
		RuleFor(x => x.CompanyName)
			.NotEmpty().WithMessage("Company Name is required.")
			.MinimumLength(1).WithMessage("Job Title is required.")
			.MaximumLength(550).WithMessage("Company Name cannot exceed 550 characters.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.JobTitle)
			.NotEmpty().WithMessage("Job Title is required.")
			.MinimumLength(1).WithMessage("Job Title is required.")
			.MaximumLength(850).WithMessage("Job Title cannot exceed 850 characters.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyEmail)
			.NotEmpty().WithMessage("Company Email is required.")
			.MinimumLength(5).WithMessage("Location must be at least 6 characters long.")
			.EmailAddress().WithMessage("A valid Company Email is required.")
			.MaximumLength(400).WithMessage("Company Email cannot exceed 400 characters.");

		RuleFor(x => x.CompanyAddress)
			.NotEmpty().MinimumLength(1).WithMessage("Address is required.")
			.MustNotHaveRepeatingCharacters(); ;

		RuleFor(x => x.CompanyPhoneNumber)
			.NotEmpty().WithMessage("Phone Number is required.")
						.Matches(@"^\d{8,10}$")
						.WithMessage("Please enter a valid phone number. It should contain 8 to 10 digits only.")
						.MaximumLength(10).WithMessage("Phone Number cannot exceed 10 characters.")
						.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyCity)
			.NotEmpty().WithMessage("City is required")
			.MinimumLength(2).WithMessage("City name must be at least 2 characters long.")
			.MaximumLength(100).WithMessage("City cannot exceed 100 characters.")
			.MustNotHaveRepeatingCharacters();

		RuleFor(x => x.CompanyLocation)
			.NotEmpty().WithMessage("Location is required")
			.MinimumLength(2).WithMessage("Location must be at least 2 characters long.")
			.MaximumLength(250).WithMessage("Location cannot exceed 250 characters.")
			.MustNotHaveRepeatingCharacters();

	}
}
