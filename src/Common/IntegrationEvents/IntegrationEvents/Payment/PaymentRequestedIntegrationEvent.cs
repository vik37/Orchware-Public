using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Payment;

[EntityName("payment-requested")]
public record PaymentRequestedIntegrationEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
	public decimal Amount { get; init; }
	public string? PaymentNumber { get; init; }
	public string? InstallmentCode { get; init; }

	public string? actionUrl { get; init; }
}
