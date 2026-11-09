using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Notification;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Constants;

namespace Orchware.Notification.API.Infrastructure.EventHandlers;

public class ManagerNotificationMessageIntegrationEventConsumer : IConsumer<ManagerNotificationMessageIntegrationEvent>
{
	private readonly INotificationService _notificationService;
	private readonly ILogger<ManagerNotificationMessageIntegrationEvent> _logger;

	public ManagerNotificationMessageIntegrationEventConsumer(INotificationService notificationService,
		ILogger<ManagerNotificationMessageIntegrationEvent> logger)
	{
		_notificationService = notificationService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<ManagerNotificationMessageIntegrationEvent> context)
	{
		try
		{
			var notification = new NotificationModel
			{
				Title = context.Message.Title,
				Message = context.Message.Message,
				ActionUrl = context.Message.ActionUrl,
				Icon = context.Message.Icon
			};

			await _notificationService.SendToRole(EmployeeRoles.Manager, notification, context.Message.Id);
			_logger.LogInformation("{Event} was proccessed successfully and message was sent to the Manager", nameof(ManagerNotificationMessageIntegrationEvent));
		}
		catch (DbUpdateException)
		{
			_logger.LogWarning("Duplicate notification blocked for MessageId: {MessageId}", context.Message.Id);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Something wrong happend {EventType} during Notification Proccessing. Message: {Message}", typeof(InvoiceSentIntegrationEvent), ex.Message);
		}
	}
}
