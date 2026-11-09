namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.Shared.Dtos;

public class PaymentRuleDto
{
	public int Id { get; set; }
	public int MaxAllowedInstallments { get; set; }
	public int GracePeriod { get; set; }

	public bool AllowDiscountModification { get; set; }
	public DiscountDescriptionDto? DiscountDescription { get; set; }
}

public class DiscountDescriptionDto
{
	public decimal MinimumOrderAmountForDiscount { get; set; }
	public decimal OrderAmountDiscountPercentage { get; set; }
	public decimal PayImmediatelyDiscountPercentage { get; set; }
}
