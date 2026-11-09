namespace Orchware.Backoffice.Application.Features.Shared.Responses;

public class FileResponse
{
	public Stream FileStream { get; set; }
	public string FileName { get; set; }
	public string ContentType { get; set; } = "application/pdf";
}
