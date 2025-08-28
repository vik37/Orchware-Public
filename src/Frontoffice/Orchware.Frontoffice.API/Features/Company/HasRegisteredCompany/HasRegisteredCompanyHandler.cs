using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Company.HasRegisteredCompany;

public class HasRegisteredCompanyHandler : IRequestHandler<HasRegisteredCompanyQuery, HasRegisteredCompanyResponse>
{
	private readonly DapperContext _dapperContext;
	private readonly IUserContextService _userContextService;

	public HasRegisteredCompanyHandler(DapperContext dapperContext, IUserContextService userContextService)
	{
		_dapperContext = dapperContext;
		_userContextService = userContextService;
	}

	public async Task<HasRegisteredCompanyResponse> Handle(HasRegisteredCompanyQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new HasRegisteredCompanyResponse();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var sql = "SELECT CompanyId FROM [dbo].[User] WHERE Id = @UserId";

		using var conn = _dapperContext.CreateConnection();
		var response = await conn.QueryFirstOrDefaultAsync<HasRegisteredCompanyResponse>(sql, new {UserId = _userContextService.Id });

		if(response is null)
			return new HasRegisteredCompanyResponse();

		if(response.CompanyId is not null)
			response.UserHasRegisteredCompany = true;

		return response;
	}
}
