using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetailsDraft;

public record GetUserCompanyDetailsDraftQuery : IRequest<GetUserCompanyDetailsDraftResponse>;
