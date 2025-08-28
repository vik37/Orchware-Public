using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.HasRegisteredCompany;

public record HasRegisteredCompanyQuery : IRequest<HasRegisteredCompanyResponse>;
