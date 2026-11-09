using MassTransit;

namespace IntegrationEvents.Orders;

[EntityName("order-requested")]
public record OrderRequestedIntegrationEvent
{
	public string OrderKey { get; init; } = string.Empty;
	public string OrderNumber { get; init; } = string.Empty;
}
