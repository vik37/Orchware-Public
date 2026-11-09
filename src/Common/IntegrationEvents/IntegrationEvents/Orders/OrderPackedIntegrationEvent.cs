using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Orders;

[EntityName("order-packed")]
public record OrderPackedIntegrationEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
}
