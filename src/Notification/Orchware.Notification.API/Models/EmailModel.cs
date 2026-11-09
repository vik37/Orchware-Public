namespace Orchware.Notification.API.Models;

public class EmailModel
{
	public string To { get; set; } = string.Empty;
	public string Subject { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	public string TextBody { get; set; }  = string.Empty;
	public string From { get; set; } = string.Empty;
	public List<string> CC { get; set; } = new();
	public List<string> BCC { get; set; } = new();
	public byte[]? AttachmentData { get; set; }
	public string? AttachmentFileName { get; set; }
}
