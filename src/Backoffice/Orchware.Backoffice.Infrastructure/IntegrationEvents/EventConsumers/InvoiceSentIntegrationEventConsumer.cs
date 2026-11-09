using IntegrationEvents.IntegrationEvents.Invoice;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.EventConsumers;

public class InvoiceSentIntegrationEventConsumer : IConsumer<InvoiceSentIntegrationEvent>
{
	private readonly ILogger<OrderRequestedIntegrationEventConsumer> _logger;
	private readonly IOrderRepository _orderRepository;

	public InvoiceSentIntegrationEventConsumer(ILogger<OrderRequestedIntegrationEventConsumer> logger,
		IOrderRepository orderRepository)
	{
		_logger = logger;
		_orderRepository = orderRepository;
	}

	public async Task Consume(ConsumeContext<InvoiceSentIntegrationEvent> context)
	{
		var order = await _orderRepository.GetByOrderNumber(context.Message.OrderNumber);

		if (order is null)
		{
			_logger.LogWarning(
					"Order with {OrderNumber} Order Number does not exist. Skipping event {EventId}",
					context.Message.OrderNumber, context.MessageId);
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
			_logger.LogInformation("Processing installment {InstallmentCode}. Skipping status change as it is not the first installment.", context.Message.InstallmentCode);
		}

		if (string.IsNullOrEmpty(context.Message.InstallmentCode))
		{
			order.PaymentInfo.SetInvoiceUrl(context.Message.InvoiceUrl);
			_logger.LogInformation("Blob URL File was successfully added to installment.");
		}
		else
		{
			order.PaymentInfo.SetInvoiceUrl(context.Message.InstallmentCode, context.Message.InvoiceUrl);
			_logger.LogInformation("Blob URL File was successfully added to payment");
		}

		await _orderRepository.UpdateAsync(order);
		await _orderRepository.UnitOfWork.SaveChangesAsync();
	}
}
