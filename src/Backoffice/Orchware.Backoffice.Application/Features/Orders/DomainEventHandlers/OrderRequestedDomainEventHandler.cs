using MediatR;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Orders.DomainEventHandlers;

public class OrderRequestedDomainEventHandler : INotificationHandler<OrderRequestedDomainEvent>
{
	private readonly IShelfRepository _shelfRepository;

	public OrderRequestedDomainEventHandler(IShelfRepository shelfRepository)
	{
		_shelfRepository = shelfRepository;
	}

	public async Task Handle(OrderRequestedDomainEvent @event, CancellationToken cancellationToken)
	{
		var productNames = @event.Order.OrderDetails.Select(x => x.Product.Name).ToList();

		var shelfDescriptions = await _shelfRepository.GetShelfInfoForProductsAsync(productNames);

		@event.Order.SetOrderDetailsShelfInfo(shelfDescriptions);
	}
}
