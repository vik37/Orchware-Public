using System.Text.Json.Serialization;

namespace Orchware.Frontoffice.API.Features.Payments.PaymentRuleDescription;

public class PaymentRuleDescriptionResponse
{
	public int MaxAllowedInstallments { get; init; }
	[JsonIgnore]
	public int MaximumPaymentPeriod { get; init; }
	public int GracePeriod { get; init; }

	public int TotalAllowedDays => MaximumPaymentPeriod + GracePeriod;

	public string FullDescription =>
		$"<b>Rules</b>: The maximum number of installments you can enter is {MaxAllowedInstallments} installments, " +
		$"due within {MaximumPaymentPeriod} days " +
		$"with <b>Grace period</b> of {GracePeriod} days.";
}
