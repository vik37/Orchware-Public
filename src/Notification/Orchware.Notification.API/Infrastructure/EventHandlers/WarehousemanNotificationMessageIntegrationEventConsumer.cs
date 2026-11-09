using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Notification;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Constants;

namespace Orchware.Notification.API.Infrastructure.EventHandlers;

public class WarehousemanNotificationMessageIntegrationEventConsumer : IConsumer<WarehousemanNotificationMessageIntegrationEvent>
{
	private readonly INotificationService _notificationService;
	private readonly ILogger<WarehousemanNotificationMessageIntegrationEventConsumer> _logger;

	public WarehousemanNotificationMessageIntegrationEventConsumer(INotificationService notificationService, 
		ILogger<WarehousemanNotificationMessageIntegrationEventConsumer> logger)
	{
		_notificationService = notificationService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<WarehousemanNotificationMessageIntegrationEvent> context)
	{
		var msg = context.Message;
		var orderNumber = msg.OrderNumber?.TrimStart('#');

		try
		{
			var notification = new NotificationModel
			{
				Title = msg.Title,
				Message = msg.Message,
				ActionUrl = $"/order/{EmployeeRoles.Warehouseman}/{orderNumber}",
				Icon = msg.Icon
			};

			await _notificationService.SendToRole(EmployeeRoles.Warehouseman, notification, msg.Id);
			_logger.LogInformation("Notification for Order {OrderNum} sent to {Role}", msg.OrderNumber, EmployeeRoles.Warehouseman);
		}
		catch (DbUpdateException)
		{
			_logger.LogWarning("Duplicate notification blocked for MessageId: {MessageId}", msg.Id);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Something wrong happend {EventType} during Notification Proccessing. Message: {Message}", typeof(InvoiceSentIntegrationEvent), ex.Message);
		}
	}
}
