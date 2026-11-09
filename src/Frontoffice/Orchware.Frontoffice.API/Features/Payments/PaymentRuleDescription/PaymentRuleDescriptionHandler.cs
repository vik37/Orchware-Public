using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Payments.PaymentRuleDescription;

public class PaymentRuleDescriptionHandler : IRequestHandler<PaymentRuleDescriptionQuery, PaymentRuleDescriptionResponse>
{
	private readonly DapperContext _dapperContext;

	public PaymentRuleDescriptionHandler(DapperContext dapperContext)
	{
		_dapperContext = dapperContext;
	}

	public async Task<PaymentRuleDescriptionResponse> Handle(PaymentRuleDescriptionQuery request, CancellationToken cancellationToken)
	{
		if(cancellationToken.IsCancellationRequested)
			return default!;

		int id = 1;
		var query = new SqlQueryBuilder();

		query.Select("TOP 1 MaxAllowedInstallments, GracePeriod, MaximumPaymentPeriod").From("[dbo].[PaymentRules]").OrderBy("Id",false);

		using var conn = _dapperContext.CreateConnection();

		var paymentRule = await conn.QueryFirstOrDefaultAsync<PaymentRuleDescriptionResponse>(query.Build(),
									new { Id = id });
		if (paymentRule is null)
			throw new NotFoundException("There is no rules for payment.");
		
		return paymentRule;
	}
}
