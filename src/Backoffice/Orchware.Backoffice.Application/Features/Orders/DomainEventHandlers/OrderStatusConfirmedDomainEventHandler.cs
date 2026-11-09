using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderStatusConfirmedDomainEventHandler : INotificationHandler<OrderConfirmedDomainEvent>
{
	private readonly IIntegrationEventProducer _integrationEventProducer;

	public OrderStatusConfirmedDomainEventHandler(IIntegrationEventProducer integrationEventProducer)
	{
		_integrationEventProducer = integrationEventProducer;
	}

	public async Task Handle(OrderConfirmedDomainEvent @event, CancellationToken cancellationToken)
	{
		await _integrationEventProducer.PublishOrderConfirmed(@event.Order.OrderNumber);
	}
}
