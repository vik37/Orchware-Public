using MediatR;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.PaymentSimulation.DomainEventHandlers;

public class PaymentConfirmedDomainEventHandler : INotificationHandler<PaymentConfirmedDomainEvent>
{
	private readonly IShelfRepository _shelfRepository;
	private readonly ILogger<PaymentConfirmedDomainEventHandler> _logger;

	public PaymentConfirmedDomainEventHandler(IShelfRepository shelfRepository, ILogger<PaymentConfirmedDomainEventHandler> logger)
	{
		_shelfRepository = shelfRepository;
		_logger = logger;
	}

	public async Task Handle(PaymentConfirmedDomainEvent @event, CancellationToken cancellationToken)
	{
		try
		{
			foreach (var item in @event.Details)
			{
				var shelf = await _shelfRepository.GetShelfByCode(item.ShelfCode);

				if (shelf is not null)
				{
					shelf.DecreaseStock(item.ShelfProductId, item.Quantity,item.UnitOfMeasure);
				}
			}

			_logger.LogInformation("Event {EventType} successfully decrease the products available quantity", typeof(PaymentConfirmedDomainEventHandler));
			await _shelfRepository.UnitOfWork.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Event {EventType} something bad happernd. Message: {ExceptionMessage}", typeof(PaymentConfirmedDomainEventHandler), ex.Message);
		}
	}
}
