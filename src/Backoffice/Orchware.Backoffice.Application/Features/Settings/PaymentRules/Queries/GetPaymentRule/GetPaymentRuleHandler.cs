using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Settings.PaymentRules.Shared.Dtos;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.Queries.GetPaymentRule;

public record GetPaymentRuleQuery : IRequest<GetPaymentRuleResponse>;

public class GetPaymentRuleHandler : IRequestHandler<GetPaymentRuleQuery, GetPaymentRuleResponse>
{
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Settings}]";

	public GetPaymentRuleHandler(
		IOptions<DatabaseConnectionOption> dbConnection, ISqlQueryBuilder sqlQueryBuilder)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<GetPaymentRuleResponse> Handle(GetPaymentRuleQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetPaymentRuleResponse();

		string sqlPaymentRuleQuery = _sqlQueryBuilder.Select("Id, MaxAllowedInstallments, GracePeriod, AllowDiscountModification")
						.From($" {Schema}.[PaymentRules]").Build();

		string sqlDiscountDescriptionQuery = _sqlQueryBuilder.Select("MinimumOrderAmountForDiscount, OrderAmountDiscountPercentage, PayImmediatelyDiscountPercentage")
						.From($" {Schema}.[PaymentRules]").Build();

		using var connection = new SqlConnection(_connectionString);

		var paymentRule = await connection.QueryFirstOrDefaultAsync<PaymentRuleDto>(sqlPaymentRuleQuery, cancellationToken);

		if (paymentRule is null)
			throw new NotFoundException("Payment Rules does not have any data");

		return new GetPaymentRuleResponse
		{
			Id = paymentRule.Id,
			MaxAllowedInstallments = paymentRule.MaxAllowedInstallments,
			GracePeriod = paymentRule.GracePeriod,
			AllowDiscountModification = paymentRule.AllowDiscountModification,
			DiscountDescription = await connection.QueryFirstOrDefaultAsync<DiscountDescriptionDto>(sqlDiscountDescriptionQuery)
		};
	}
}
