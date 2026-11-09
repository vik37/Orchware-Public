using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrders;

[Route("api/get-orders")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
public class GetOrdersController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetOrdersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Retreives list of Orders, ordered by Logged User.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/get-orders
	///	  
	/// 
	/// </example>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="Exception"></exception>
	/// <returns>Order Response Details including List of Orders</returns>
	[HttpGet]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCompanyById(CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetOrdersQuery(), token));
	}
}
