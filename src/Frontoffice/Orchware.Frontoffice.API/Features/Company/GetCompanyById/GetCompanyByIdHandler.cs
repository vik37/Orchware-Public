using AutoMapper;
using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Company.GetCompanyById;

public class GetCompanyByIdHandler : IRequestHandler<GetCompanyByIdQuery, GetCompanyByIdResponse>
{
	private readonly DapperContext _dapperContext;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private readonly IMapper _mapper;
	private readonly IUserContextService _userContextService;

	public GetCompanyByIdHandler(DapperContext dapperContext, ISqlQueryBuilder sqlQueryBuilder, IMapper mapper,
		IUserContextService userContextService)
	{
		_dapperContext = dapperContext;
		_mapper = mapper;
		_sqlQueryBuilder = sqlQueryBuilder;
		_userContextService = userContextService;
	}

	public async Task<GetCompanyByIdResponse> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetCompanyByIdResponse();

		if (request.Id <= 0)
			throw new BadRequestException("Invalid Request.");

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var queryBuilder = _sqlQueryBuilder
								.Select("top 1 c.[Id], c.[Name], c.[Email], c.[City], c.[Location], c.[Address], c.[Phone]")
								.From("[dbo].[Company] c")
								.Join("INNER", "[dbo].[User] u", "u.CompanyId = c.Id")
								.Where("c.Id = @CompanyId AND u.Id = @UserId")
								.Build();


		using var conn = _dapperContext.CreateConnection();

		var company = await conn.QueryFirstOrDefaultAsync<Domain.Company>(queryBuilder, 
									new { CompanyId = request.Id, UserId =  _userContextService.Id});

		if (company is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		return _mapper.Map<GetCompanyByIdResponse>(company);
	}
}
