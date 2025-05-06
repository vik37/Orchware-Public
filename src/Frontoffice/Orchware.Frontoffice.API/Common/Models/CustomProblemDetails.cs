using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Common.Models;

public class CustomProblemDetails : ProblemDetails
{
	public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
