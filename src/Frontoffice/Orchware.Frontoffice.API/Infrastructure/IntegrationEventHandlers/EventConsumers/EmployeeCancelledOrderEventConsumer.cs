using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class EmployeeCancelledOrderEventConsumer : IConsumer<EmployeeCancelledOrderEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<EmployeeCancelledOrderEventConsumer> _logger;

	public EmployeeCancelledOrderEventConsumer(OrchwareDbContext dbContext,
				ILogger<EmployeeCancelledOrderEventConsumer> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<EmployeeCancelledOrderEvent> context)
	{
		if (context.Message == null || string.IsNullOrWhiteSpace(context.Message.OrderNumber))
		{
			_logger.LogWarning(
				"Skipping {Event}. Message or OrderNumber is empty.",
				nameof(EmployeeCancelledOrderEvent));

			return;
		}

		var order = await _dbContext.Order.Include(x => x.User).FirstOrDefaultAsync(x => x.OrderNumber == context.Message.OrderNumber);

		if (order == null)
		{
			_logger.LogWarning(
				"Order not found while processing {Event}. OrderNumber: {OrderNumber}",
				nameof(EmployeeCancelledOrderEvent),
				context.Message.OrderNumber);

			return;
		}

		if (order.Status == OrderStatus.Cancelled)
		{
			_logger.LogInformation(
				"Order {OrderNumber} already cancelled. Skipping {Event}.",
				order.OrderNumber,
				nameof(EmployeeCancelledOrderEvent));

			return;
		}

		order.SetStatus(OrderStatus.Cancelled);

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"{Event} processed successfully. Order {OrderNumber} cancelled for user {User}.",
			nameof(EmployeeCancelledOrderEvent),
			order.OrderNumber,
			order.User.Name);
	}
}
