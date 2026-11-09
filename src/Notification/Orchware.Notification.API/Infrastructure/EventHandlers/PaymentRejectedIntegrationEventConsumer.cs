using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.IntegrationEvents.Payment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Constants;

namespace Orchware.Notification.API.Infrastructure.EventHandlers;

public class PaymentRejectedIntegrationEventConsumer : IConsumer<PaymentRejectedIntegrationEvent>
{
	private readonly INotificationService _notificationService;
	private readonly INotifySender _sender;
	private readonly ILogger<ManagerNotificationMessageIntegrationEvent> _logger;

	public PaymentRejectedIntegrationEventConsumer(INotificationService notificationService,
		INotifySender sender, ILogger<ManagerNotificationMessageIntegrationEvent> logger)
	{
		_notificationService = notificationService;
		_logger = logger;
		_sender = sender;
	}

	public async Task Consume(ConsumeContext<PaymentRejectedIntegrationEvent> context)
	{
		try
		{
			var notification = new NotificationModel
			{
				Title = context.Message.Title,
				Message = context.Message.Message,
				ActionUrl = context.Message.ActionURL,
				Icon = "rectangle-xmark"
			};

			await _notificationService.SendToRole(EmployeeRoles.Manager, notification, context.Message.Id);
			_logger.LogInformation("{Event} was proccessed successfully and message was sent to the Manager", nameof(PaymentRejectedIntegrationEvent));

			var email = new EmailModel
			{
				To = context.Message.CustomerEmail,
				Subject = context.Message.Title,
				Body = GenerateHTML(context.Message.Message, context.Message.Customer, context.Message.PaymentDueDate)
			};

			await _sender.SendEmailAsync(email);
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

	private string GenerateHTML(string message, string customer, DateTime? paymentDueDate)
	{
		return $@"
			<!DOCTYPE html>
					<html>
					<head>
						<style>
							body {{ font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; padding: 20px; }}
							.container {{ max-width: 600px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border-top: 5px solid #d9534f; }}
							.header {{ font-size: 22px; font-weight: bold; color: #d9534f; margin-bottom: 20px; }}
							.message {{ margin-bottom: 25px; }}
							.alert-box {{ background-color: #fff5f5; border-left: 4px solid #d9534f; padding: 15px; margin: 20px 0; border-radius: 4px; }}
							.footer {{ margin-top: 30px; font-size: 12px; color: #999; border-top: 1px solid #eee; padding-top: 20px; text-align: center; }}
							strong {{ color: #2c3e50; }}
						</style>
					</head>
					<body>
						<div class=""container"">
							<div class=""header"">Payment Info</div>
        
							<p>Dear {customer},</p>
        
							<p class=""message""> We would like to inform you that {message}</p>
        
							<p>Please check your financial account status or contact your account manager so we can resolve this matter and continue processing your order.</p>
        
							{(paymentDueDate.HasValue ? $"<div class='alert-box'><b>Important:</b> The final payment due date is <b>{paymentDueDate:MMMM dd, yyyy}</b></div>" : "")}
        
							<p>Thank you for your understanding.</p>
        
							<p>Best regards,<br>
							<strong>ORCHWARE Team</strong></p>
        
							<div class=""footer"">
								<p>This is an automated notification. Please do not reply directly to this email.</p>
							</div>
						</div>
					</body>
					</html>
	
		";
	}
}
