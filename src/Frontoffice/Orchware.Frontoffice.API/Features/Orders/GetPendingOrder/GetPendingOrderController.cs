using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.GetPendingOrder;

[Route("api/order-pending")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
public class GetPendingOrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetPendingOrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Retrieves the currently pending (pre-confirmed) order for the logged-in user, 
	/// including its details.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   GET: api/order-pending
	///   
	/// </example>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <returns>Pre-confirmed {panding) orders, including their details</returns>
	[HttpGet]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateOrder(CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetPendingOrderQuery(), token));
	}
}
