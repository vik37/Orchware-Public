using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Orchware.Frontoffice.API.Features
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private static readonly ActivitySource activitySource = new("orchware-frontoffice-api");

		[HttpGet("trace-test")]
		public IActionResult TraceTest()
		{
			using var activity = activitySource.StartActivity("ManualTestTrace");

			activity?.SetTag("custom.tag", "test_value");

			return Ok("Manual span sent");
		}
	}
}
