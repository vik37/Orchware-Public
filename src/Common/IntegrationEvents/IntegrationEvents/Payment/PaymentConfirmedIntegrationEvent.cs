using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Payment;

[EntityName("payment-confirmed")]
public record PaymentConfirmedIntegrationEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
	public string? PaymentNumber { get; init; }
	public string? InstallmentCode { get; init; }
	public string ActionUrl { get; set; } = string.Empty;

	public string Customer { get; init; } = string.Empty;
	public string CustomerEmail { get; init; } = string.Empty;
}
