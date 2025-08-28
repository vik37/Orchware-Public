using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.GetCompanyById;

public record GetCompanyByIdQuery(int Id) : IRequest<GetCompanyByIdResponse>;
