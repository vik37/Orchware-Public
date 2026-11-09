using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Payment;

[EntityName("payment-rejected")]
public record PaymentRejectedIntegrationEvent
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Title { get; init; } = string.Empty;
	public string Message { get; init; } = string.Empty;
	public string ActionURL { get; init; } = string.Empty;

	public string Customer { get; init; } = string.Empty;
	public string CustomerEmail { get; init; } = string.Empty;
	public DateTime? PaymentDueDate { get; set; }
}
