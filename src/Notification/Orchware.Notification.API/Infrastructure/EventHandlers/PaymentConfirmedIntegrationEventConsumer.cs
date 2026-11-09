using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.IntegrationEvents.Payment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Constants;

namespace Orchware.Notification.API.Infrastructure.EventHandlers;

public class PaymentConfirmedIntegrationEventConsumer : IConsumer<PaymentConfirmedIntegrationEvent>
{
	private readonly INotificationService _notificationService;
	private readonly INotifySender _sender;
	private readonly ILogger<ManagerNotificationMessageIntegrationEvent> _logger;

	public PaymentConfirmedIntegrationEventConsumer(INotificationService notificationService, INotifySender sender,
		ILogger<ManagerNotificationMessageIntegrationEvent> logger)
	{
		_notificationService = notificationService;
		_sender = sender;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<PaymentConfirmedIntegrationEvent> context)
	{
		try
		{
			var message = context.Message;

			string notificationMessage = string.IsNullOrEmpty(message.InstallmentCode)
											? $"Payment <b>{message.PaymentNumber ?? "N/A"}</b> has been received for Order <b>{message.OrderNumber}</b>."
											: $"Installment <b>{message.InstallmentCode}</b> has been received for Order <b>{message.OrderNumber}</b>.";

			var notification = new NotificationModel
			{
				Title = $"Payment Confirmation - Order {message.OrderNumber}",
				Message = notificationMessage,
				ActionUrl = message.ActionUrl,
				Icon = "cc-amazon-pay"
			};

			await _notificationService.SendToRole(EmployeeRoles.Manager, notification, context.Message.MessageId);
			_logger.LogInformation("{Event} was proccessed successfully and message was sent to the Manager", nameof(PaymentConfirmedIntegrationEvent));

			string paymentNumber = message.PaymentNumber ?? "N/A";
			var body = GenerateHTML(message.Customer, message.OrderNumber, paymentNumber, message.InstallmentCode);

			var email = new EmailModel
			{
				To = context.Message.CustomerEmail,
				Subject = notification.Title,
				Body = body
			};

			await _sender.SendEmailAsync(email);
		}
		catch (DbUpdateException)
		{
			_logger.LogWarning("Duplicate notification blocked for MessageId: {MessageId}", context.MessageId);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Something wrong happend {EventType} during Notification Proccessing. Message: {Message}", typeof(PaymentConfirmedIntegrationEvent), ex.Message);
		}
	}

	private string GenerateHTML(string customer, string orderNumber, string paymentNumber, string? installmentCode = null)
	{
		return $@"
			<!DOCTYPE html>
                    <html>
                        <head>
                            <style>
                                body {{ font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; padding: 20px; }}
                                .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border-top: 5px solid #28a745; }}
                                .header {{ font-size: 22px; font-weight: bold; color: #28a745; margin-bottom: 20px; }}
                                .details {{ background-color: #f8fff9; padding: 15px; border-radius: 8px; border: 1px solid #d4edda; margin: 20px 0; }}
                                .footer {{ margin-top: 30px; font-size: 12px; color: #999; border-top: 1px solid #eee; padding-top: 20px; text-align: center; }}
                            </style>
                        </head>
                        <body>
                            <div class=""container"">
                                <div class=""header"">Payment Confirmed</div>
        
                                <p>Dear {customer},</p>
        
                                <p>We are pleased to inform you that your payment has been successfully processed. Your order is now moving to the next stage.</p>
        
                                <div class=""details"">
                                    <p style=""margin: 5px 0;""><strong>Order Number:</strong> {orderNumber}</p>
                                    {(string.IsNullOrEmpty(paymentNumber) ? "" : $"<p style='margin: 5px 0;'><strong>Payment Reference:</strong> {paymentNumber}</p>")}
                                    {(string.IsNullOrEmpty(installmentCode) ? "" : $"<p style='margin: 5px 0;'><strong>Installment Plan:</strong> {installmentCode}</p>")}
                                </div>
        
                                <p>Thank you for your business. If you have any questions, please feel free to reach out to our support team.</p>
        
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
