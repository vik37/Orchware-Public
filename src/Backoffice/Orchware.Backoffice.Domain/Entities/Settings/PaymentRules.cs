using Orchware.Backoffice.Domain.Events;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Settings;

public class PaymentRules : AggregateRoot<int>
{
	public int MaxAllowedInstallments { get; private set; }
	public TimeSpan GracePeriod { get; private set; }
	public TimeSpan MaximumPaymentPeriod => TimeSpan.FromDays(30);

	// What is the minimum total order amount for which it would receive a discount?
	public decimal MinimumOrderAmountForDiscount { get; private set; }

	// What is the discount if a larger order is placed on the total amount of a single order?
	public decimal OrderAmountDiscountPercentage { get; private set; }

	// What discount will be applied if the entire amount is paid at once?
	public decimal PayImmediatelyDiscountPercentage { get; private set; }

	public bool AllowDiscountModification { get; private set; } = false;

	public PaymentRules()
	{}

	public PaymentRules(int maxAllowedInstallments, TimeSpan gracePeriod,
						decimal minimumOrderAmountForDiscount, decimal orderAmountDiscountPercentage, decimal payImmediatelyDiscountPercentage)
	{
		this.MaxAllowedInstallments = maxAllowedInstallments;
		this.GracePeriod = gracePeriod;
		this.MinimumOrderAmountForDiscount = minimumOrderAmountForDiscount;
		this.OrderAmountDiscountPercentage = orderAmountDiscountPercentage;
		this.PayImmediatelyDiscountPercentage = payImmediatelyDiscountPercentage;
	}

	public void Update(int maxAllowedInstallments, int gracePeriod,
		bool allowDiscountModification)
	{
		this.MaxAllowedInstallments = maxAllowedInstallments;
		this.GracePeriod = TimeSpan.FromDays(gracePeriod);

		this.AllowDiscountModification = allowDiscountModification;


		this.AddDomainEvent(new PaymentRuleChangedDomainEvent(this));
	}

	public void UpdateDiscount(decimal minimumOrderAmountForDiscount,
								decimal orderAmountDiscountPercentage,
								decimal payImmediatelyDiscountPercentage)
	{
		this.MinimumOrderAmountForDiscount = minimumOrderAmountForDiscount;
		this.OrderAmountDiscountPercentage = orderAmountDiscountPercentage;
		this.PayImmediatelyDiscountPercentage = payImmediatelyDiscountPercentage;
	}
}
