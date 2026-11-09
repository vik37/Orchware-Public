using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Orders;

[EntityName("employee-cancelled-order")]
public record EmployeeCancelledOrderEvent
{
	public Guid MessageId { get; init; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
}
