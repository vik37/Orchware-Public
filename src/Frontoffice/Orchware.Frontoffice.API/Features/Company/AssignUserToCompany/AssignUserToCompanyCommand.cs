using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.AssignUserToCompany;

public record AssignUserToCompanyCommand : IRequest<string>
{
	public int CompanyId { get; init; }
	public string JobTitle { get; init; } = string.Empty;
}
