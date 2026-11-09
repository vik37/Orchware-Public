using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Orders;

[EntityName("order-approved")]
public record OrderApprovedIntegrationEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
}
