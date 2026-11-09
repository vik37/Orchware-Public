using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Notification;

[EntityName("manager-notification-message-sent")]
public record ManagerNotificationMessageIntegrationEvent
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; init; } = string.Empty;
	public string Message { get; init; } = string.Empty;
	public string ActionUrl { get; init; } = string.Empty;

	public string OrderNumber { get; init; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
}
