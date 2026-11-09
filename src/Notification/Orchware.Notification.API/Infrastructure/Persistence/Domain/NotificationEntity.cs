namespace Orchware.Notification.API.Infrastructure.Persistence.Domain;

public class NotificationEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public string? ActionUrl { get; set; }
	public string? TargetRole { get; set; }
	public string? TargetUserId { get; set; }
	public string? TargetGroup { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public bool IsRead { get; set; } = false;
	public string? Icon { get; set; }

	public Guid ExternalMeesageId { get; set; }
}
