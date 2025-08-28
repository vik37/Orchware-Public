using MediatR;

namespace Orchware.Frontoffice.API.Features.Company.EditUserJobTitle;

public record EditUserCompanyJobTitleCommand(int CompanyId,string JobTitle) : IRequest<Unit>;
