using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Orders.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Unit>
{
	private readonly OrchwareDbContext _context;
	private readonly ILogger<CancelOrderHandler> _logger;
	private readonly IPublishEndpoint _publishEndpointProvider;

	public CancelOrderHandler(OrchwareDbContext context, ILogger<CancelOrderHandler> logger,
		IPublishEndpoint publishEndpointProvider)
	{
		_context = context;
		_logger = logger;
		_publishEndpointProvider = publishEndpointProvider;
	}

	public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
	{
		var order = await _context.Order.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

		if (order is null)
			throw new NotFoundException("Order was not found!!!");
		
		order.SetStatus(OrderStatus.Cancelled);

		await _context.SaveChangesAsync(cancellationToken);

		await _publishEndpointProvider.Publish(new CustomerCancelledOrderEvent { OrderNumber = order.OrderNumber, Reason = request.Reason }, cancellationToken);

		_logger.LogInformation("Published {EventType} for order {OrderNumber}",
									nameof(CustomerCancelledOrderEvent), order.OrderNumber);
		return Unit.Value;
	}
}
