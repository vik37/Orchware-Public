using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.PendingOrderCreation;

[Route("api/pending-order-creation")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
[EnableRateLimiting("slide-by-ip")]
public class PendingOrderCreationController : ControllerBase
{
	private readonly IMediator _mediator;

	public PendingOrderCreationController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Creates or updates the temporary pending order for the logged-in user, 
	/// including its details. The order is stored temporarily until it is submitted.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   POST: api/pending-order-creation
	///   
	/// </example>
	/// <param name="command">Pending Order Creation Command</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="BadRequestException"></exception>
	/// <returns>No Content</returns>
	[HttpPost]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateOrder([FromBody] PendingOrderCreationCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}
}
