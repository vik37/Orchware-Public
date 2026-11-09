using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Orders;

[EntityName("customer-cancelled-order")]
public record CustomerCancelledOrderEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
	public string Reason { get; init; } = string.Empty;
}
