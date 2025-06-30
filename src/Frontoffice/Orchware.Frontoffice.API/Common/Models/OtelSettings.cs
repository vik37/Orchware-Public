namespace Orchware.Frontoffice.API.Common.Models;

public class OtelSettings
{
	public string Endpoint { get; set; } = string.Empty;
	public string Protocol { get; set; } = "grpc";
}
