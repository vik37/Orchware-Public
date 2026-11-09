using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Storage.Blobs;
using IntegrationEvents.IntegrationEvents.Invoice;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Infrastructure.AzCosmos;
using Orchware.Notification.API.Infrastructure.AzCosmos.Entity;
using Orchware.Notification.API.Models;
using System.Collections.Concurrent;

namespace Orchware.Notification.API.Controllers;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
	private readonly INotifySender _sender;
	private readonly IOrderCosmosRepository _orderCosmosRepository;
	private readonly ILogger<EmailController> _logger;
	private readonly IPublishEndpoint _publishEndpoint;

	private static readonly ConcurrentDictionary<string, byte> _activeProcessingOrders = new();

	public EmailController(INotifySender sender, IOrderCosmosRepository orderCosmosRepository, ILogger<EmailController> logger,
		IPublishEndpoint publishEndpoint)
	{
		_sender = sender;
		_orderCosmosRepository = orderCosmosRepository;
		_logger = logger;
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost("listener")]
	public async Task<IActionResult> EventGridListener([FromBody] EventGridEvent[] events)
	{
		foreach (var e in events)
		{
			if (e.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
			{
				var validationData = e.Data.ToObjectFromJson<SubscriptionValidationEventData>();

				_logger.LogInformation("Handshake received with code: {Code}", validationData.ValidationCode);

				return Ok(new
				{
					validationResponse = validationData.ValidationCode
				});
			}

			if (e.EventType == "Microsoft.Storage.BlobCreated")
			{
				var blobData = e.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
				string fileName = Path.GetFileName(new Uri(blobData.Url).LocalPath);
				string orderNumber = ParseOrderNumberFromFileName(Path.GetFileName(blobData.Url));

				if (!_activeProcessingOrders.TryAdd(orderNumber, 0))
				{
					_logger.LogWarning("Duplicate EventGrid request received for order {Num}. Thread locked and skipped.", orderNumber);
					continue;
				}

				try
				{
					var order = await _orderCosmosRepository.GetOrderByOrderNumber(orderNumber);

					if (order == null || order.Buyer == null || order.Invoice?.EmailSent == true)
					{
						_logger.LogInformation("Order {Num} already processed or not found. Skipping.", orderNumber);
						continue;
					}

					int retryCount = 0;
					while (string.IsNullOrWhiteSpace(order.Invoice?.PdfUrl) && retryCount < 5)
					{
						_logger.LogWarning("Order {OrderNumber} has no PdfUrl yet. Waiting for PDF generation...", orderNumber);
						await Task.Delay(1000);
						order = await _orderCosmosRepository.GetOrderByOrderNumber(orderNumber);
					}

					if (string.IsNullOrWhiteSpace(order.Invoice?.PdfUrl))
					{
						_logger.LogWarning("Order {OrderNumber} still has no PdfUrl after waiting. Skipping.", orderNumber);
						continue;
					}

					try
					{
						await _orderCosmosRepository.MarkEmailAsSent(order.Id, order.OrderNumber);

						var blobUri = new Uri(order.Invoice.PdfUrl);
						var blobClient = new BlobClient(blobUri);

						var downloadInfo = await blobClient.DownloadContentAsync();
						byte[] pdfBytes = downloadInfo.Value.Content.ToArray();

						string? installmentCode = order.PaymentInfo?.Installments?.InstallmentCode;

						await _sender.SendEmailAsync(new EmailModel
						{
							To = order.Buyer.PersonalEmail,
							Subject = $"Invoice for Order {order.OrderNumber}",
							Body = GenerateInvoiceEmailBody(order.Buyer, order.OrderNumber, order.Invoice.PdfUrl, order.OrderedDate, installmentCode),
							CC = new List<string> { "manager@viktor-showcase.dev" },
							AttachmentData = pdfBytes,
							AttachmentFileName = Path.GetFileName(blobUri.LocalPath)
						});

						if (order.PaymentInfo?.Installments != null && order.PaymentInfo.NumberOfInstallments >= 1)
						{
							await _publishEndpoint.Publish(new InvoiceSentIntegrationEvent
							{
								OrderNumber = order.OrderNumber,
								InvoiceUrl = blobData.Url,
								InstallmentCode = installmentCode
							});
						}
						else
						{
							await _publishEndpoint.Publish(new InvoiceSentIntegrationEvent
							{
								OrderNumber = order.OrderNumber,
								InvoiceUrl = blobData.Url
							});
						}

						_logger.LogInformation("Invoice email sent and confirmed for order {orderNumber}", orderNumber);
						await _orderCosmosRepository.DeleteOrderAsync(order.Id, order.OrderNumber);
					}
					catch (Exception emailOrPublishEx)
					{
						_logger.LogError(emailOrPublishEx, "Email dispatch or publishing failed for {orderNumber}. Rolling back Cosmos status. Error: {Message}", orderNumber, emailOrPublishEx.Message);

						try
						{
							await _orderCosmosRepository.ResetEmailStatus(order.Id, order.OrderNumber);
						}
						catch (Exception rollbackEx)
						{
							_logger.LogError(rollbackEx, "Failed to rollback email status for order {orderNumber}", orderNumber);
						}

						throw;
					}
				}
				catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
				{
					_logger.LogWarning("Thread lost the race for order {orderNumber}. Skipping duplicate.", orderNumber);
				}
				catch (Exception generalEx)
				{
					_logger.LogError(generalEx, "Unexpected error processing EventGrid webhook for order {orderNumber}: {Message}", orderNumber, generalEx.Message);
				}
				finally
				{
					_activeProcessingOrders.TryRemove(orderNumber, out _); 
				}
			}
		}

		return Ok();
	}

	private string ParseOrderNumberFromFileName(string fileName)
	{
		try
		{
			var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

			var parts = nameWithoutExt.Split('_');

			if (parts.Length >= 2)
				return "#" + parts[1];
		}
		catch (Exception ex)
		{
			_logger.LogError("Error parsing fileName: {Message}", ex.Message);
		}
		return null;
	}

	private string GenerateInvoiceEmailBody(Buyer buyer, string orderNumber, string pdfUrl, DateTime orderedDate, string? numberOfInstallments = null)
	{
		string formattedDate = orderedDate.ToString("MMM dd, yyyy");
		string fullTimestamp = orderedDate.ToString("f");

		string paymentTypeText = "Full Payment";
		if (!string.IsNullOrEmpty(numberOfInstallments))
		{
			paymentTypeText = $"Installment {numberOfInstallments} of {numberOfInstallments}";
		}

		return $@"
				<html>
					<head>
						<style>
							@media only screen and (max-width: 600px) {{
								.container {{ width: 100% !important; border-radius: 0 !important; }}
								.content {{ padding: 20px !important; }}
							}}
						</style>
					</head>
					<body style='font-family: ""Inter"", ""Segoe UI"", Helvetica, Arial, sans-serif; line-height: 1.6; color: #2d3436; background-color: #f0f2f5; padding: 40px 10px; margin: 0;'>
						<div class='container' style='max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
            
							<div style='background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%); padding: 35px 20px; text-align: center;'>
								<h2 style='margin: 0; color: #ffffff; font-size: 24px; font-weight: 400; letter-spacing: 1px;'>Invoice for Order {orderNumber}</h2>
								<p style='color: #bdc3c7; margin: 10px 0 0 0; font-size: 14px;'>Issued on {formattedDate} • <strong style='color: #3498db;'>{paymentTypeText}</strong></p>
							</div>
            
							<div class='content' style='padding: 40px;'>
								<div style='margin-bottom: 30px;'>
									<span style='font-size: 11px; text-transform: uppercase; color: #3498db; font-weight: bold; letter-spacing: 1px;'>Recipient Details</span>
									<h3 style='margin: 5px 0 0 0; font-size: 20px;'>{buyer.Name}</h3>
									<p style='margin: 2px 0; color: #636e72;'>{buyer.JobTitle} • <a href='mailto:{buyer.PersonalEmail}' style='color: #3498db; text-decoration: none;'>{buyer.PersonalEmail}</a></p>
								</div>

								<div style='background-color: #f8f9fa; border-radius: 10px; padding: 25px; border: 1px solid #edf2f7;'>
									<div style='margin-bottom: 15px; border-bottom: 1px solid #e2e8f0; padding-bottom: 10px;'>
										<span style='font-size: 11px; text-transform: uppercase; color: #718096; font-weight: bold;'>Billing To</span>
										<p style='margin: 5px 0; font-weight: bold; font-size: 16px; color: #2d3748;'>{buyer.CompanyName}</p>
									</div>

									<table style='width: 100%; border-collapse: collapse;'>
										<tr>
											<td style='padding: 5px 0; vertical-align: top;'>
												<span style='color: #718096; font-size: 13px;'>Address:</span><br/>
												<span style='font-size: 14px;'>{buyer.CompanyAddress}</span><br/>
												<span style='font-size: 14px;'>{buyer.CompanyCity}, {buyer.CompanyLocation}</span>
											</td>
											<td style='padding: 5px 0; text-align: right; vertical-align: top;'>
												<span style='color: #718096; font-size: 13px;'>Contact:</span><br/>
												<span style='font-size: 14px;'>{buyer.CompanyEmail}</span><br/>
												<span style='font-size: 14px;'>+389{buyer.CompanyPhoneNumber}</span>
											</td>
										</tr>
									</table>
								</div>

								<div style='text-align: center; margin: 40px 0 10px 0;'>
									<a href='{pdfUrl}' style='background-color: #3498db; color: #ffffff; padding: 16px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 15px; display: inline-block; box-shadow: 0 4px 6px rgba(52, 152, 219, 0.2);'>
										Download PDF Invoice
									</a>
								</div>

								<p style='text-align: center; font-size: 12px; color: #b2bec3; margin-top: 20px;'>
									Order date: {fullTimestamp}
								</p>

								<div style='margin-top: 40px; padding-top: 20px; border-top: 1px solid #edf2f7; text-align: center;'>
									<p style='font-size: 12px; color: #a0aec0;'>
										Questions? Email us at <a href='mailto:info@viktor-showcase.dev' style='color: #3498db; text-decoration: none;'>info@viktor-showcase.dev</a>
									</p>
								</div>
							</div>

							<div style='background-color: #2d3436; padding: 20px; text-align: center;'>
								<p style='margin: 0; font-size: 11px; color: #95a5a6; text-transform: uppercase; letter-spacing: 1px;'>
									Generated by Orchware Backoffice
								</p>
							</div>
						</div>
					</body>
				</html>";
	}
}
