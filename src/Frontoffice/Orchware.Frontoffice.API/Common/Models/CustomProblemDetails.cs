using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Common.Models;

public class CustomProblemDetails : ProblemDetails
{
	public string? EntityName { get; set; }
	public string? Identifier { get; set; }
	public int? ExistingEntityId { get; set; }
	public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
