using MediatR;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderPreparationStartedDomainEventHandler : INotificationHandler<OrderPreparationStartedDomainEvent>
{
	private readonly ILogger<OrderPreparationStartedDomainEventHandler> _logger;

	public OrderPreparationStartedDomainEventHandler(ILogger<OrderPreparationStartedDomainEventHandler> logger)
	{
		_logger = logger;
	}

	public Task Handle(OrderPreparationStartedDomainEvent notification, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Domain Logic: Order {Order Number} has now officially begun preparation and packaging.", notification.OrderNumber);

		return Task.CompletedTask;
	}
}
