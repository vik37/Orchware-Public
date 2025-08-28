using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetails;

public record GetUserCompanyDetailsQuery(int CompanyId) : IRequest<GetUserCompanyDetailsResponse>{}
