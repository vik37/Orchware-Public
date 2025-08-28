using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.UpdateCompany;

public record UpdateCompanyCommand : IRequest<Unit>
{
	public int CompanyId { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string CompanyEmail { get; init; } = string.Empty;
	public string CompanyAddress { get; init; } = string.Empty;
	public string CompanyPhoneNumber { get; init; } = string.Empty;
	public string CompanyCity { get; set; } = string.Empty;
	public string CompanyLocation { get; set; } = string.Empty;
}
