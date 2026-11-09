namespace Orchware.Frontoffice.API.Domain;

public class PaymentRules : BaseInt
{
	public int MaxAllowedInstallments { get; set; }
	public TimeSpan GracePeriod { get; set; }
	public TimeSpan MaximumPaymentPeriod { get; set; }

	// What is the minimum total order amount for which it would receive a discount?
	public decimal MinimumOrderAmountForDiscount { get; set; }

	// What is the discount if a larger order is placed on the total amount of a single order?
	public decimal OrderAmountDiscountPercentage { get; set; }

	// What discount will be applied if the entire amount is paid at once?
	public decimal PayImmediatelyDiscountPercentage { get; set; }
}
