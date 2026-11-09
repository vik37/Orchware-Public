using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrderDetails;

[Route("api/get-order-details")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
public class GetOrderDetailsController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetOrderDetailsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// [TEMP] Retrieves a order details by order ID for an authenticated user. 
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/get-order-details/{orderId}
	///	  
	/// 
	/// </example>
	/// <param name="token">Cancellation token</param>
	/// <param name="OrderId"></param>
	/// <exception cref="Exception"></exception>
	/// <returns>Order Response Details for particular order</returns>
	[HttpGet("{OrderId}")]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrderDetails(string OrderId, CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetOrderDetailsQuery(OrderId)));
	}
}
