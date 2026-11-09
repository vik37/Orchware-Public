using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class InvoiceSentIntegrationEventConsumer : IConsumer<InvoiceSentIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderConfirmedIntegrationEvent> _logger;

	public InvoiceSentIntegrationEventConsumer(OrchwareDbContext dbContext,
												ILogger<OrderConfirmedIntegrationEvent> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<InvoiceSentIntegrationEvent> context)
	{
		if (context.Message == null || string.IsNullOrWhiteSpace(context.Message.OrderNumber))
		{
			_logger.LogWarning(
				"Skipping {Event}. Message or OrderNumber is empty.",
				nameof(InvoiceSentIntegrationEvent));

			return;
		}

		var order = await _dbContext.Order
			.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.OrderNumber == context.Message.OrderNumber);

		if (order == null)
		{
			_logger.LogError(
				"Order not found while processing {Event}. OrderNumber: {OrderNumber}",
				nameof(InvoiceSentIntegrationEvent),
				context.Message.OrderNumber);

			return;
		}

		bool shouldChangeStatus = string.IsNullOrEmpty(context.Message.InstallmentCode) ||
								   context.Message.InstallmentCode.EndsWith("-01");

		if (shouldChangeStatus)
		{
			if (order.Status != OrderStatus.InPreparation)
				order.SetStatus(OrderStatus.InPreparation);
		}
		else
		{
			_logger.LogInformation(
				"Processing installment {InstallmentCode} for Order {OrderNumber}. Skipping status change as it is not the first installment.",
				context.Message.InstallmentCode,
				order.OrderNumber);
		}

		var payment = await _dbContext.Payment.Include(x => x.Installments).FirstOrDefaultAsync(x => x.OrderId == order.Id);

		if (payment is null)
		{
			var errorMsg = $"CRITICAL: Payment record missing for Order {order.OrderNumber} (ID: {order.Id}).";
			_logger.LogError(errorMsg);

			return;
		}

		if(string.IsNullOrEmpty(context.Message.InstallmentCode))
			payment.SetInvoiceUrl(context.Message.InvoiceUrl);
		else
			payment.SetInvoiceUrl(context.Message.InstallmentCode, context.Message.InvoiceUrl);

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"{Event} processed successfully. Order {OrderNumber} confirmed for user {User}.",
			nameof(InvoiceSentIntegrationEvent),
			order.OrderNumber,
			order.User.Name);
	}
}
