using IntegrationEvents.IntegrationEvents.Invoice;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;

namespace Orchware.Notification.API.Infrastructure.EventHandlers;

public class InvoiceSentIntegrationEventConsumer : IConsumer<InvoiceSentIntegrationEvent>
{
	private readonly INotificationService _notificationService;
	private readonly ILogger<InvoiceSentIntegrationEvent> _logger;

	public InvoiceSentIntegrationEventConsumer(INotificationService notificationService, ILogger<InvoiceSentIntegrationEvent> logger)
	{
		_notificationService = notificationService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<InvoiceSentIntegrationEvent> context)
	{
		try
		{
			var orderNumber = context.Message.OrderNumber?.TrimStart('#');
			var notification = new NotificationModel
			{
				Title = "New order for preparation.",
				Message = "Good morning, warehouse workers. We just received a new order for preparation. Please have one of you pick it up and start packing. Order at the URL below.",
				ActionUrl = $"/order/warehouseman/{orderNumber}",
				Icon = "people-carry-box"
			};

			await _notificationService.SendToRole("warehouseman", notification, context.Message.Id);

			_logger.LogInformation("Notification for Order {OrderNum} sent to Warehouse", context.Message.OrderNumber);
		}
		catch (DbUpdateException ex)
		{
			_logger.LogWarning("Prevent duplication message {Message}", ex.Message);
		}
		catch(Exception ex)
		{
			_logger.LogWarning(ex,"Something wrong happend {EventType} during Notification Proccessing. Message: {Message}", typeof(InvoiceSentIntegrationEvent), ex.Message);
		}
	}
}
