using IntegrationEvents.IntegrationEvents.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Entities.Orders;
using System.Net.Http.Json;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.EventConsumers;

public class PaymentConfirmedIntegrationEventConsumer : IConsumer<PaymentConfirmedIntegrationEvent>
{
	private readonly ILogger<OrderRequestedIntegrationEventConsumer> _logger;
	private readonly IOrderRepository _orderRepository;
	private readonly ExternalURLS _externalUrls;

	public PaymentConfirmedIntegrationEventConsumer(ILogger<OrderRequestedIntegrationEventConsumer> logger,
		IOrderRepository orderRepository, IOptions<ExternalURLS> options)
	{
		_logger = logger;
		_orderRepository = orderRepository;
		_externalUrls = options.Value;
	}

	public async Task Consume(ConsumeContext<PaymentConfirmedIntegrationEvent> context)
	{
		var message = context.Message;

		var order = await _orderRepository.GetByOrderNumber(message.OrderNumber, context.CancellationToken);

		if (!string.IsNullOrEmpty(message.InstallmentCode))
		{
			order!.PaymentInfo!.MarkInstallmentAsPaid(message.InstallmentCode);

			string lastPart = message.InstallmentCode.Split('-').Last();

			var url = order.PaymentInfo?.Installments?.FirstOrDefault(x => x.InstallmentCode == message.InstallmentCode)?.InvoiceUrl;

			if (lastPart == "01")
				order.AddPaymentConfirmedForThisOrderDomainEvent();

			if (!string.IsNullOrEmpty(url))
				await SendInvoiceStampRequest(order.OrderNumber, url);

			_logger.LogInformation("Proccessing Event {Event} completed: Installment {Code} marked as paid for Payment {Number}", typeof(PaymentConfirmedIntegrationEvent), message.InstallmentCode, message.PaymentNumber);
		}
		else
		{
			order!.PaymentInfo!.MarkAsPaid();
			order.AddPaymentConfirmedForThisOrderDomainEvent();

			if(order is not null && order.PaymentInfo is not null && !string.IsNullOrEmpty(order.PaymentInfo.InvoiceUrl))
				await SendInvoiceStampRequest(order.OrderNumber, order.PaymentInfo.InvoiceUrl!);

			_logger.LogInformation("Proccessing Event {Event} completed: Payment {Number} marked as paid", typeof(PaymentConfirmedIntegrationEvent), message.PaymentNumber);
		}

		await _orderRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
	}

	private async Task SendInvoiceStampRequest(string orderNumber, string url)
	{
		string urlFunc = $"{_externalUrls.StampFunctionUrl}={_externalUrls.StampFunctionCode}==";

		using (var http = new HttpClient())
		{
			var response = await http.PostAsJsonAsync(urlFunc, new InvoiceStampRequest
			{
				OrderNumber = orderNumber,
				BlobPath = url
			});

			response.EnsureSuccessStatusCode();
		}
	}
}

public class InvoiceStampRequest
{
	public string OrderNumber { get; set; } = string.Empty;
	public string BlobPath { get; set; } = string.Empty;
}