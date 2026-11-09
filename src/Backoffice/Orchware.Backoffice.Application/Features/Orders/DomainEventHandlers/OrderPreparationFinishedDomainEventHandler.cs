using MediatR;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderPreparationFinishedDomainEventHandler : INotificationHandler<OrderPreparationFinishedDomainEvent>
{
	private readonly IIntegrationEventProducer _integrationEventProducer;
	private readonly ILogger<OrderPreparationFinishedDomainEventHandler> _logger;

	public OrderPreparationFinishedDomainEventHandler(IIntegrationEventProducer integrationEventProducer,
		ILogger<OrderPreparationFinishedDomainEventHandler> logger)
	{
		_integrationEventProducer = integrationEventProducer;
		_logger = logger;
	}

	public async Task Handle(OrderPreparationFinishedDomainEvent @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Domain Logic: Order {Order Number} has now officially finished preparation and packaging.", @event.OrderNumber);
		await _integrationEventProducer.PublishOrderPacked(@event.OrderNumber);

		var title = $"Order {@event.OrderNumber} – has been packed.";
		var message = $"Order {@event.OrderNumber} preparation has been completed, and the order is packed. It is awaiting verification and further processing.";

		await _integrationEventProducer.PublishManagerNotificationMessage(@event.OrderId, title, message, @event.OrderNumber, "box");
	}
}

