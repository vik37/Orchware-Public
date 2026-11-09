using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderApprovedDomainEventHandler : INotificationHandler<OrderApprovedDomainEvent>
{
	private readonly IIntegrationEventProducer _integrationEventProducer;

	public OrderApprovedDomainEventHandler(IIntegrationEventProducer integrationEventProducer)
	{
		_integrationEventProducer = integrationEventProducer;
	}

	public async Task Handle(OrderApprovedDomainEvent @event, CancellationToken cancellationToken)
	{
		await _integrationEventProducer.PublishOrderApproved(@event.OrderNumber);

		var title = $"Order {@event.OrderNumber} – Approved for Dispatch";
		var message = $"The order {@event.OrderNumber} is approved for shipment. Please proceed with loading and hand over to the courier for delivery.";

		await _integrationEventProducer.PublishWarehousemanNotificationMessage(title, message, @event.OrderNumber, "truck-loading");
	}
}
