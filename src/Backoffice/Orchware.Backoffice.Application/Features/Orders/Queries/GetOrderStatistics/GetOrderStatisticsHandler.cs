using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using System.Data;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrderStatistics;

public record GetOrderStatisticsQuery : IRequest<GetOrderStatisticsResponse>;

public class GetOrderStatisticsHandler : IRequestHandler<GetOrderStatisticsQuery, GetOrderStatisticsResponse>
{
	private readonly string _connectionString;
	private const string Schema = $"[{DomainSchema.Order}]";

	public GetOrderStatisticsHandler(IOptions<DatabaseConnectionOption> dbConnection)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
	}

	public async Task<GetOrderStatisticsResponse> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetOrderStatisticsResponse();

		using var connection = new SqlConnection(_connectionString);

		using var multi = await connection.QueryMultipleAsync(
											$"{Schema}.[SP_GetOrderStatistics]",
											commandType: CommandType.StoredProcedure);

		var response = new GetOrderStatisticsResponse
		{
			Overall = await multi.ReadFirstOrDefaultAsync<OverallStatsDto>() ?? new(),
			Companies = await multi.ReadAsync<CompanyStatsDto>(),
			Buyers = await multi.ReadAsync<BuyerCompanyStatsDto>(),
			Payments = await multi.ReadFirstOrDefaultAsync<PaymentStatsDto>() ?? new(),
			Cancellations = await multi.ReadAsync<CancellationStatsDto>(),
			TopProductByQuantity = await multi.ReadFirstOrDefaultAsync<ProductStatsDto>() ?? new(),
			TopProductByOrders = await multi.ReadFirstOrDefaultAsync<ProductStatsDto>() ?? new(),
			UniqueCounts = await multi.ReadFirstOrDefaultAsync<UniqueCountsStatsDto>() ?? new(),
			RepeatBuyers = await multi.ReadAsync<BuyerCompanyStatsDto>(),
			Monthly = await multi.ReadAsync<MonthlyStatsDto>(),
			CashFlow = await multi.ReadAsync<CashFlowForecastDto>(),
			OverdueCompanies = await multi.ReadAsync<OverdueCompanyStatsDto>()
		};

		return response;
	}
}
