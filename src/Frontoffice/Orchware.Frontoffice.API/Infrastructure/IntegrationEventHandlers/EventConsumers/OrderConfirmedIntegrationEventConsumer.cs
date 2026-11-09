using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class OrderConfirmedIntegrationEventConsumer : IConsumer<OrderConfirmedIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderConfirmedIntegrationEventConsumer> _logger;

	public OrderConfirmedIntegrationEventConsumer(OrchwareDbContext dbContext,
							ILogger<OrderConfirmedIntegrationEventConsumer> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<OrderConfirmedIntegrationEvent> context)
	{
		if (context.Message == null || string.IsNullOrWhiteSpace(context.Message.OrderNumber))
		{
			_logger.LogWarning(
				"Skipping {Event}. Message or OrderNumber is empty.",
				nameof(OrderConfirmedIntegrationEvent));

			return;
		}

		var order = await _dbContext.Order
			.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.OrderNumber == context.Message.OrderNumber);

		if (order == null)
		{
			_logger.LogWarning(
				"Order not found while processing {Event}. OrderNumber: {OrderNumber}",
				nameof(OrderConfirmedIntegrationEvent),
				context.Message.OrderNumber);

			return;
		}

		if (order.Status == OrderStatus.Confirmed)
		{
			_logger.LogInformation(
				"Order {OrderNumber} already confirmed. Skipping {Event}.",
				order.OrderNumber,
				nameof(OrderConfirmedIntegrationEvent));

			return;
		}

		order.SetStatus(OrderStatus.Confirmed);

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"{Event} processed successfully. Order {OrderNumber} confirmed for user {User}.",
			nameof(OrderConfirmedIntegrationEvent),
			order.OrderNumber,
			order.User.Name);
	}
}
