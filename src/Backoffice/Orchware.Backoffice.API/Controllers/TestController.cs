using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;

namespace Orchware.Backoffice.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private readonly IUserContextService _userContextService;

		public TestController(IUserContextService userContextService)
		{
			_userContextService = userContextService;
		}

		[HttpGet("user")]
		//[Authorize(Roles = "user")]
		public IActionResult UserTest()
		{
			var user = new
			{
				Id = _userContextService.Id,
				Username = _userContextService.UserName,
				Email = _userContextService.Email,
				Firtname = _userContextService.Firstname,
				Lastname = _userContextService.Lastname,
				Role = _userContextService.Role
			};

			return Ok(user);
		}

		[HttpGet("user/claims")]
		[Authorize]
		public IActionResult UserClaims()
		{

			var claims = User.Claims
						.GroupBy(c => c.Type)
						.ToDictionary(
							g => g.Key,
							g => g.Select(c => c.Value).ToList()
						);

			return Ok(new
			{
				isAuthenticated = User.Identity?.IsAuthenticated,
				name = User.Identity?.Name,
				claims = claims
			});
		}
	}
}
