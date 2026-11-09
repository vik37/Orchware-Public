namespace Orchware.Backoffice.Application.Features.Shared.Responses;

public class SelectedListResponse
{
	public string Column { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public List<string> Values { get; set; } = new List<string>();
}
