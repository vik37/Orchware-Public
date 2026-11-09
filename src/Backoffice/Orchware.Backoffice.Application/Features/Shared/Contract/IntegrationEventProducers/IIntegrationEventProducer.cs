
namespace Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;

public interface IIntegrationEventProducer
{
	Task PublishOrderConfirmed(string orderNumber);
	Task PublishOrderPacked(string orderNumber);
	Task PublishOrderApproved(string orderNumber);
	Task PublishOrderCancelled(string orderNumber);

	Task PublishWarehousemanNotificationMessage(string title, string message, string orderNumber, string icon);
	Task PublishManagerNotificationMessage(Guid orderId, string title, string message, string orderNumber, string icon);

	Task PublishPaymentRequest(string orderNumber, decimal amount, string? paymentNumber = null, string? installmentCode = null, string? actionUrl = null);

	Task PaymentRuleChanged(int maxAllowedInstallments, TimeSpan gracePeriod, TimeSpan maximumPaymentPeriod,decimal minimumOrderAmountForDiscount,
								decimal orderAmountDiscountPercentage, decimal payImmediatelyDiscountPercentage);
}
