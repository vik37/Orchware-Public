using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class OrderApprovedIntegrationEventConsumer : IConsumer<OrderApprovedIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderApprovedIntegrationEventConsumer> _logger;

	public OrderApprovedIntegrationEventConsumer(OrchwareDbContext dbContext,
													ILogger<OrderApprovedIntegrationEventConsumer> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<OrderApprovedIntegrationEvent> context)
	{
		if (context.Message == null || string.IsNullOrWhiteSpace(context.Message.OrderNumber))
		{
			_logger.LogWarning(
				"Skipping {Event}. Message or OrderNumber is empty.",
				nameof(OrderConfirmedIntegrationEvent));

			return;
		}

		var order = await _dbContext.Order.Include(x => x.User).FirstOrDefaultAsync(x => x.OrderNumber == context.Message.OrderNumber);

		if(order == null)
		{
			_logger.LogWarning(
				"Order not found while processing {Event}. OrderNumber: {OrderNumber}",
				nameof(OrderApprovedIntegrationEvent),
				context.Message.OrderNumber);

			return;
		}

		if (order.Status == OrderStatus.Approved)
		{
			_logger.LogInformation(
				"Order {OrderNumber} already approved. Skipping {Event}.",
				order.OrderNumber,
				nameof(OrderApprovedIntegrationEvent));

			return;
		}

		order.SetStatus(OrderStatus.Approved);

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"{Event} processed successfully. Order {OrderNumber} approved for user {User}.",
			nameof(OrderApprovedIntegrationEvent),
			order.OrderNumber,
			order.User.Name);
	}
}
