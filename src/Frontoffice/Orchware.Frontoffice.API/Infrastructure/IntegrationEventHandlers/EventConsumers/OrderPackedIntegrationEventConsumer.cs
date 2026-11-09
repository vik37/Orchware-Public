using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class OrderPackedIntegrationEventConsumer : IConsumer<OrderPackedIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderPackedIntegrationEventConsumer> _logger;

	public OrderPackedIntegrationEventConsumer(OrchwareDbContext dbContext,
		ILogger<OrderPackedIntegrationEventConsumer> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<OrderPackedIntegrationEvent> context)
	{
		if (context.Message == null || string.IsNullOrWhiteSpace(context.Message.OrderNumber))
		{
			_logger.LogWarning(
				"Skipping {Event}. Message or OrderNumber is empty.",
				nameof(OrderPackedIntegrationEvent));

			return;
		}

		var order = await _dbContext.Order
			.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.OrderNumber == context.Message.OrderNumber);

		if (order == null)
		{
			_logger.LogWarning(
				"Order not found while processing {Event}. OrderNumber: {OrderNumber}",
				nameof(OrderPackedIntegrationEvent),
				context.Message.OrderNumber);

			return;
		}

		if (order.Status == OrderStatus.AwaitingDispatch)
		{
			_logger.LogInformation(
				"Order {OrderNumber} already packed. Skipping {Event}.",
				order.OrderNumber,
				nameof(OrderPackedIntegrationEvent));

			return;
		}

		order.SetStatus(OrderStatus.AwaitingDispatch);

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"{Event} processed successfully. Order {OrderNumber} awaiting to dispatch for user {User}.",
			nameof(OrderConfirmedIntegrationEvent),
			order.OrderNumber,
			order.User.Name);
	}
}
