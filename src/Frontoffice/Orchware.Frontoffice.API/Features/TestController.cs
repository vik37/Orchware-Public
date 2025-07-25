﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.Contracts;
using System.Diagnostics;

namespace Orchware.Frontoffice.API.Features
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private static readonly ActivitySource activitySource = new("orchware-frontoffice-api");

		private readonly IUserContextService _userContextService;

		public TestController(IUserContextService userContextService)
		{
			_userContextService = userContextService;
		}

		[HttpGet("trace-test")]
		public IActionResult TraceTest()
		{
			using var activity = activitySource.StartActivity("ManualTestTrace");

			activity?.SetTag("custom.tag", "test_value");

			return Ok("Manual span sent");
		}

		[HttpGet("user")]
		[Authorize(Policy = "NonEmployeeUser")]
		public IActionResult UserTest()
		{
			var user = new
			{
				Id = _userContextService.Id,
				Username = _userContextService.UserName,
				Email = _userContextService.Email,
				Firtname = _userContextService.Firstname,
				Lastname = _userContextService.Lastname
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
