namespace Orchware.Notification.API.Settings.Models;

public class RabbitMQSettings
{
	public string Host { get; set; } = string.Empty;
	public string VHost { get; set; } = string.Empty;
	public ushort Port { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}
