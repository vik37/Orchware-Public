using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.RequestOrder;

[Route("api/request-order")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class RequestOrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public RequestOrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Submits a request for a order.
	/// </summary>
	/// <remarks>
	/// This endpoint allows a client to place an order request. The request is
	/// stored in the system and is automatically dispatched as an integration event
	/// to the back-office for review and approval by a manager. The manager will then
	/// either confirm the request and generate a formal offer, or decline it. No immediate
	/// payment is processed at this stage.
	/// </remarks>
	/// <example>
	///   Request:
	///   
	///   POST: api/api/request-order
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
	public async Task<IActionResult> CreateOrder([FromBody] RequestOrderCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}
}
