using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.CancelOrder;

[Route("api/cancel-order")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
[EnableRateLimiting("slide-by-ip")]
public class CancelOrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public CancelOrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Client cancle the order.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   POST: api/cancel-order
	///   
	/// </example>
	/// <param name="command">Pending Order Creation Command</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="BadRequestException"></exception>
	/// <returns>No Content</returns>
	[HttpPut]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CancleOrder(CancelOrderCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);

		return NoContent();
	}
}
