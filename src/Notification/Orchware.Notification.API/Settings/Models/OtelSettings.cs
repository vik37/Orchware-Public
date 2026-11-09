namespace Orchware.Notification.API.Settings.Models;

public class OtelSettings
{
	public string Endpoint { get; set; } = string.Empty;
	public string Protocol { get; set; } = "grpc";
}
