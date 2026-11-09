using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Application.Features.Shared.Responses;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.OrderBuyerSelectedList;

public record OrderBuyerSelectedListQuery : IRequest<List<SelectedListResponse>>;

public class OrderBuyerSelectedListHandler : IRequestHandler<OrderBuyerSelectedListQuery, List<SelectedListResponse>>
{
	private readonly IUserContextService _userContextService;
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Order}]";

	public OrderBuyerSelectedListHandler(IUserContextService userContextService,
		IOptions<DatabaseConnectionOption> dbConnection, ISqlQueryBuilder sqlQueryBuilder)
	{
		_userContextService = userContextService;
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<List<SelectedListResponse>> Handle(OrderBuyerSelectedListQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new List<SelectedListResponse>();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var sqlBuilder = _sqlQueryBuilder.Select("DISTINCT Buyer_Name, Buyer_CompanyName")
											.From($"{Schema}.[Order]")
											.Build();

		using var connection = new SqlConnection(_connectionString);

		var data = (await connection.QueryAsync(sqlBuilder)).ToList();

		var buyerNames = data
			.Select(x => (string)x.Buyer_Name)
			.Where(x => !string.IsNullOrEmpty(x))
			.Distinct()
			.ToList();

		var companyNames = data
			.Select(x => (string)x.Buyer_CompanyName)
			.Where(x => !string.IsNullOrEmpty(x))
			.Distinct()
			.ToList();

		var result = new List<SelectedListResponse>
		{
			new SelectedListResponse
			{
				Column = "Buyer_Name",
				Title = "Buyer",
				Values = buyerNames
			},
			new SelectedListResponse
			{
				Column = "Buyer_CompanyName",
				Title = "Company",
				Values = companyNames
			}
		};

		return result;
	}
}
