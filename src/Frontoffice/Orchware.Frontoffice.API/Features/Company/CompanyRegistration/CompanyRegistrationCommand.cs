using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistration;

public record CompanyRegistrationCommand : IRequest<int>
{
	public string CompanyName { get; init; } = string.Empty;
	public string JobTitle { get; init; } = string.Empty;
	public string CompanyEmail { get; init; } = string.Empty;
	public string CompanyAddress { get; init; } = string.Empty;
	public string CompanyPhoneNumber { get; init; } = string.Empty;
	public string CompanyCity { get; init; } = string.Empty;
	public string CompanyLocation { get; init; } = string.Empty;
}
