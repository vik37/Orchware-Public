using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistrationDraft;

public record CompanyRegistrationDraftCommand : IRequest<Unit>
{
	public string? CompanyName { get; init; }
	public string? JobTitle { get; init; }
	public string? CompanyEmail { get; init; }
	public string? CompanyAddress { get; init; }
	public string? CompanyPhoneNumber { get; init; }
	public string? CompanyCity { get; init; }
	public string? CompanyLocation { get; init; }
}
