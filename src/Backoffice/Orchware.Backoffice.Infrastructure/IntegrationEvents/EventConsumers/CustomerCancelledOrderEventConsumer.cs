using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Enums;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.EventConsumers;

public class CustomerCancelledOrderEventConsumer : IConsumer<CustomerCancelledOrderEvent>
{
	private readonly ILogger<CustomerCancelledOrderEvent> _logger;
	private readonly IOrderRepository _orderRepository;

	public CustomerCancelledOrderEventConsumer(ILogger<CustomerCancelledOrderEvent> logger,
				IOrderRepository orderRepository)
	{
		_logger = logger;
		_orderRepository = orderRepository;
	}

	public async Task Consume(ConsumeContext<CustomerCancelledOrderEvent> context)
	{
		var order = await _orderRepository.GetByOrderNumber(context.Message.OrderNumber);

		if (order is null)
		{
			_logger.LogWarning(
					"Order with {OrderNumber} Order Number does not exist. Skipping event {EventId}",
					context.Message.OrderNumber, context.MessageId);
			return;
		}

		string previousStatus = order.Status.Value;

		order.CancelOrder(context.Message.Reason, CancelledBy.Client);

		await _orderRepository.UpdateAsync(order);
		await _orderRepository.UnitOfWork.SaveChangesAsync();

		_logger.LogInformation("Order {OrderNumber} successfully processed {Event}",
			order.OrderNumber, nameof(CustomerCancelledOrderEvent));

		var title = $"Order {order.OrderNumber} – has been cancelled.";
		var message = $"Order {order.OrderNumber} has been cancelled by {order.Buyer.Name} ({order.Buyer.CompanyName}). " +
			  $"Please check the order details and the reason for cancellation to determine the next steps regarding the client and payment.";

		await context.Publish(new ManagerNotificationMessageIntegrationEvent
		{
			Title = title,
			Message = message,
			OrderNumber = order.OrderNumber,
			ActionUrl = $"/order/manager/{order.Id}",
			Icon = "handshake-slash"
		});

		if (previousStatus != "Requested" && previousStatus != "Confirmed")
		{
			message = $"Order {order.OrderNumber} has been cancelled by the client. Please, stop any activities related to this order.";

			await context.Publish(new WarehousemanNotificationMessageIntegrationEvent
			{
				Title = title,
				Message = message,
				OrderNumber = order.OrderNumber,
				Icon = "store-slash"
			});
		}
	}
}
