using Orchware.Backoffice.Application.Features.Settings.PaymentRules.Shared.Dtos;

namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.Queries.GetPaymentRule;

public class GetPaymentRuleResponse : PaymentRuleDto
{
	public int MaximumPaymentPeriod => 30;
}
