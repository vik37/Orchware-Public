using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Backoffice.Application.Features.Settings.PaymentRules.Commands.EditPaymentRule;
using Orchware.Backoffice.Application.Features.Settings.PaymentRules.Queries.GetPaymentRule;

namespace Orchware.Backoffice.API.Controllers;

[Route("api/settings")]
[ApiController]
public class SettingsController : ControllerBase
{
	private readonly IMediator _mediator;

	public SettingsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	#region PaymentRule

	/// <summary>
	/// Retrieves Payment Rule Description (Data).
	/// </summary>
	/// <param name="token"></param>
	/// <returns>PaymentRuleDto data including Discount Dto Description intended for manager</returns>
	[HttpGet("payment-rule")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetPaymentRule(CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetPaymentRuleQuery(), token));
	}

	/// <summary>
	/// Edit Payment Rule Data.
	/// </summary>
	/// <param name="command"></param>
	/// <param name="token"></param>
	[HttpPut("payment-rule")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetPaymentRule(EditPaymentRuleCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}

	#endregion
}
