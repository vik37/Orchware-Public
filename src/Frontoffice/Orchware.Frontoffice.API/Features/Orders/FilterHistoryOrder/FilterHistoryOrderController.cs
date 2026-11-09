using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.FilterHistoryOrder;

[Route("api/history-orders")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
public class FilterHistoryOrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public FilterHistoryOrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Handles the retrieval of filtered and paginated order history for the currently logged-in user.
	/// Supports optional filtering by order status ('Cancelled' or 'Approved') and a date range (From/To), 
	/// returning a paginated result containing order details, aggregated amounts, and total count.
	/// </summary>
	///   Request:
	///   
	///	  POST: 	api/history-orders
	///	  
	/// 
	/// </example>
	/// <param name="filter"></param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="Exception"></exception>
	/// <returns>List of History Order Response </returns>
	[HttpPost]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> FilterOrderHistory(FilterHistoryOrderRequest filter, CancellationToken token)
	{
		return Ok(await _mediator.Send(new FilterHistoryOrderCommand(filter), token));
	}
}
