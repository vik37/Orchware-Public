using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Notification;

[EntityName("warehouseman-notification-message-sent")]
public record WarehousemanNotificationMessageIntegrationEvent
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; init; } = string.Empty;
	public string Message { get; init; } = string.Empty;
	public string OrderNumber { get; init; } = string.Empty;

	public string Icon { get; set; } = string.Empty;
}
