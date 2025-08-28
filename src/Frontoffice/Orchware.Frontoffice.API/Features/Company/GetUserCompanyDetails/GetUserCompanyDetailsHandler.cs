using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetails;

public class GetUserCompanyDetailsHandler : IRequestHandler<GetUserCompanyDetailsQuery, GetUserCompanyDetailsResponse>
{
	private readonly DapperContext _dapperContext;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private readonly IUserContextService _userContextService;

	public GetUserCompanyDetailsHandler(DapperContext dapperContext, ISqlQueryBuilder sqlQueryBuilder,
		IUserContextService userContextService)
	{
		_dapperContext = dapperContext;
		_sqlQueryBuilder = sqlQueryBuilder;
		_userContextService = userContextService;
	}

	public async Task<GetUserCompanyDetailsResponse> Handle(GetUserCompanyDetailsQuery request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (request.CompanyId <= 0)
			throw new BadRequestException("Invalid Request.");

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var sqlQuery = _sqlQueryBuilder
			.Select("TOP 1 u.Id, u.[Name], u.PersonalEmail, u.CompanyId, u.JobTitle, " +
					"c.[Name] as CompanyName, c.[Email] as CompanyEmail, c.City AS CompanyCity, c.Location AS CompanyLocation, c.Address AS CompanyAddress, c.Phone AS CompanyPhone, " +
					"c.CreatedDate, c.ModifiedDate")
			.From("[dbo].[User] u")
			.Join("INNER", "[dbo].[Company] c", "u.CompanyId = @CompanyId")
			.Where("u.Id = @UserId and c.Id = @CompanyId")
			.Build();

		using var conn = _dapperContext.CreateConnection();

		var response = await conn.QueryFirstOrDefaultAsync<GetUserCompanyDetailsResponse>(sqlQuery, 
									new { UserId = _userContextService.Id, CompanyId = request.CompanyId});

		if (response == null)
			throw new ForbiddenException("You do not have access to this company's details.");

		return response;
	}
}
