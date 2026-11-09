namespace Orchware.Notification.API.Models;

public class NotificationModel
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public string? ActionUrl { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public bool IsRead { get; set; } = false;

	public string? Icon { get; set; }
}
