using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using System.Data;

namespace Orchware.Backoffice.Application.Features.PaymentSimulation.Queries;

public record GetPaymentInfoForOrderQuery : IRequest<IEnumerable<GetPaymentInfoForOrderResponse>>;

public class GetPaymentInfoForOrderQueryHandler : IRequestHandler<GetPaymentInfoForOrderQuery, IEnumerable<GetPaymentInfoForOrderResponse>>
{
	private readonly string _connectionString;
	private string Schema = $"[{DomainSchema.Order}]";

	public GetPaymentInfoForOrderQueryHandler(IOptions<DatabaseConnectionOption> dbConnection)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
	}

	public async Task<IEnumerable<GetPaymentInfoForOrderResponse>> Handle(GetPaymentInfoForOrderQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new List<GetPaymentInfoForOrderResponse>();
		
		var orderDictionary = new Dictionary<string, GetPaymentInfoForOrderResponse>();

		using var connection = new SqlConnection(_connectionString);

		await connection.QueryAsync<GetPaymentInfoForOrderResponse, InstallmentsDto, GetPaymentInfoForOrderResponse>(
				$"{Schema}.[SP_GetPaymentTrackingData]",
			(order, installment) =>
			{
				if (!orderDictionary.TryGetValue(order.OrderNumber, out var orderEntry))
				{
					orderEntry = order;
					orderEntry.Installments = new List<InstallmentsDto>();
					orderDictionary.Add(orderEntry.OrderNumber, orderEntry);
				}

				if (installment != null && !string.IsNullOrEmpty(installment.Code))
				{
					orderEntry.Installments.Add(installment);
				}

				return orderEntry;
			},
			splitOn: "Code",
			commandType: CommandType.StoredProcedure
		);

		return orderDictionary.Values;
	}
}
