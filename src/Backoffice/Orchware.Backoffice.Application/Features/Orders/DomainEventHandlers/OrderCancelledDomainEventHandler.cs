using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderCancelledDomainEventHandler : INotificationHandler<OrderCancelledDomainEvent>
{
	private readonly IIntegrationEventProducer _integrationEventProducer;

	public OrderCancelledDomainEventHandler(IIntegrationEventProducer integrationEventProducer)
	{
		_integrationEventProducer = integrationEventProducer;
	}

	public async Task Handle(OrderCancelledDomainEvent @event, CancellationToken cancellationToken)
	{
		await _integrationEventProducer.PublishOrderCancelled(@event.OrderNumber);

		if(@event.PreviousStatus != "Requested" && @event.PreviousStatus != "Confirmed")
		{
			var title = $"Order {@event.OrderNumber} – has been cancelled.";
			var message = $"Order {@event.OrderNumber} has been cancelled by the manager. Please, stop any activities related to this order.";
			await _integrationEventProducer.PublishWarehousemanNotificationMessage(title, message, @event.OrderNumber, "ban");
		}
	}
}
