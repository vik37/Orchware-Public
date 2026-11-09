using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Orders;

[EntityName("order-confirmed")]
public record OrderConfirmedIntegrationEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
}
