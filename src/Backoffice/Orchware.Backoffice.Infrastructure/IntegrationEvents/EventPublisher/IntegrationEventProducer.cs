using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.IntegrationEvents.Orders;
using IntegrationEvents.IntegrationEvents.Payment;
using IntegrationEvents.IntegrationEvents.PaymentRules;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.EventPublisher;

public class IntegrationEventProducer : IIntegrationEventProducer
{
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly ILogger<IntegrationEventProducer> _logger;

	public IntegrationEventProducer(IPublishEndpoint publishEndpoint, ILogger<IntegrationEventProducer> logger)
	{
		_publishEndpoint = publishEndpoint;
		_logger = logger;
	}

	public async Task PublishOrderApproved(string orderNumber)
	{
		await _publishEndpoint.Publish(new OrderApprovedIntegrationEvent { OrderNumber = orderNumber });
		_logger.LogInformation("Published {EventType} for order {OrderNumber}",
									nameof(OrderApprovedIntegrationEvent), orderNumber);
	}

	public async Task PublishOrderCancelled(string orderNumber)
	{
		await _publishEndpoint.Publish(new EmployeeCancelledOrderEvent { OrderNumber = orderNumber });
		_logger.LogInformation("Published {EventType} for order {OrderNumber}",
									nameof(EmployeeCancelledOrderEvent), orderNumber);
	}

	public async Task PublishOrderConfirmed(string orderNumber)
	{
		await _publishEndpoint.Publish(new OrderConfirmedIntegrationEvent { OrderNumber = orderNumber });
		_logger.LogInformation("Published {EventType} for order {OrderNumber}",
									nameof(OrderConfirmedIntegrationEvent), orderNumber);
	}

	public async Task PublishOrderPacked(string orderNumber)
	{
		await _publishEndpoint.Publish(new OrderPackedIntegrationEvent { OrderNumber = orderNumber });
		_logger.LogInformation("Published {EventType} for order {OrderNumber}",
									nameof(OrderPackedIntegrationEvent), orderNumber);
	}

	public async Task PublishWarehousemanNotificationMessage(string title, string message, string orderNumber, string icon)
	{
		await _publishEndpoint.Publish(new WarehousemanNotificationMessageIntegrationEvent
		{
			Title = title,
			Message = message,
			OrderNumber = orderNumber,
			Icon = icon
		});

		_logger.LogInformation("Notification Message published for Order {OrderNumber} to Warehouseman", orderNumber);
	}

	public async Task PublishManagerNotificationMessage(Guid orderId, string title, string message, string orderNumber, string icon)
	{
		await _publishEndpoint.Publish(new ManagerNotificationMessageIntegrationEvent
		{
			Title = title,
			Message = message,
			OrderNumber = orderNumber,
			ActionUrl = $"/order/manager/{orderId}",
			Icon = icon
		});

		_logger.LogInformation("Notification Message published for Order {OrderNumber} to Manager", orderNumber);
	}

	public async Task PublishPaymentRequest(string orderNumber, decimal amount, string? paymentNumber = null, string? installmentCode = null,
		string? actionUrl = null)
	{
		await _publishEndpoint.Publish(new PaymentRequestedIntegrationEvent { OrderNumber = orderNumber, Amount = amount, PaymentNumber = paymentNumber, 
			InstallmentCode = installmentCode, actionUrl = actionUrl });

		_logger.LogInformation("Payment Simulation Request was Published for Order {OrderNumber}", orderNumber);
	}

	public async Task PaymentRuleChanged(int maxAllowedInstallments, TimeSpan gracePeriod, TimeSpan maximumPaymentPeriod, 
		decimal minimumOrderAmountForDiscount, decimal orderAmountDiscountPercentage, decimal payImmediatelyDiscountPercentage)
	{
		await _publishEndpoint.Publish(new PaymentRuleChangedIntegrationEvent
		{
			MaxAllowedInstallments = maxAllowedInstallments,
			GracePeriod = gracePeriod,
			MaximumPaymentPeriod = maximumPaymentPeriod,
			MinimumOrderAmountForDiscount = minimumOrderAmountForDiscount,
			OrderAmountDiscountPercentage = orderAmountDiscountPercentage,
			PayImmediatelyDiscountPercentage = payImmediatelyDiscountPercentage
		});

		_logger.LogInformation("{Event} successfulyy published", nameof(PaymentRuleChangedIntegrationEvent));
	}
}
