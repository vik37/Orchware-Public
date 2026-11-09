using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Backoffice.Application.Features.Orders.Commands;
using Orchware.Backoffice.Application.Features.Orders.Commands.FilterHistoryOrder;
using Orchware.Backoffice.Application.Features.Orders.Queries.GetOrderInvoicePDF;
using Orchware.Backoffice.Application.Features.Orders.Queries.GetOrders;
using Orchware.Backoffice.Application.Features.Orders.Queries.GetOrderStatistics;
using Orchware.Backoffice.Application.Features.Orders.Queries.ManagerOrderDetails;
using Orchware.Backoffice.Application.Features.Orders.Queries.OrderBuyerSelectedList;
using Orchware.Backoffice.Application.Features.Orders.Queries.OrderStatuses;
using Orchware.Backoffice.Application.Features.Orders.Queries.WarehousemanOrderDetails;

namespace Orchware.Backoffice.API.Controllers;

[Route("api/order")]
[ApiController]
public class OrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public OrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Retrieves a list of orders filtered by status for the logged-in user.
	/// </summary>
	/// <remarks>
	/// Pagination is supported only for Manager users when retrieving all orders.
	/// Pagination is applied only when the total number of orders exceeds 10.
	/// Warehouseman users retrieve orders without pagination.
	/// </remarks>
	/// <param name="status">
	/// The status to filter the orders by (e.g., 'Requested', 'InPreparation').
	/// Pagination can be provided for Manager users when the status is 'All'.
	/// </param>
	/// <param name="pageIndex">
	/// Pagination Page Index
	/// </param>
	/// <param name="pageSize">
	/// Pgination Page Size
	/// </param>
	/// <param name="token"></param>
	/// <returns>
	/// Order response details including the list of orders and total order count.
	/// </returns>
	[HttpGet]
	[Authorize(Roles = "manager, warehouseman")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrdersByStatus([FromQuery] string status = "All", 
		[FromQuery] int? pageIndex = null, [FromQuery] int? pageSize = null, 
		CancellationToken token = default)
	{
		return Ok(await _mediator.Send(new GetOrdersQuery(new GetOrdersRequest { Status = status, pageIndex = pageIndex, pageSize = pageSize}), token));
	}

	/// <summary>
	/// Retrieves a list of history orders paginated & filtered by logged-in user.
	/// </summary>
	/// <param name="command">Pagination/Filter History Order Command.</param>
	/// <param name="token"></param>
	/// <returns>Order Response Details including List of Orders</returns>
	[HttpPost("history")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetHistoryOrders([FromBody] FilterHistoryOrderCommand command, CancellationToken token)
	{
		return Ok(await _mediator.Send(command, token));
	}

	/// <summary>
	/// Retrieves a list of all order statuses.
	/// </summary>
	/// <param name="token"></param>
	/// <returns>List of Order Statuses</returns>
	[HttpGet("all-statuses")]
	[Authorize(Roles = "manager, warehouseman")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrderStatuses(CancellationToken token)
	{
		return Ok(await _mediator.Send(new OrderStatusesQuery(), token));
	}

	/// <summary>
	/// Confirms an existing order by order number.
	/// Only users with the Manager role are allowed to perform this action.
	/// </summary>
	/// <param name="orderId">OrderID for particular Order.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>No content if the order is successfully confirmed.</returns>
	[HttpPut("confirm/{orderId}")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-command")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> OrderConfirm([FromRoute] Guid orderId, CancellationToken token)
	{
		await _mediator.Send(new ConfirmOrderCommand(orderId), token);
		return NoContent();
	}

	/// <summary>
	/// Retrieves Order with his details intended for Warehouseman Role.
	/// </summary>
	/// <param name="orderNumber">OrderNumber for particular Order.</param>
	/// <param name="token"></param>
	/// <returns>Order Detaails Response by Id intended for Warehousman user role</returns>
	[HttpGet("warehouseman-orderdetails/{orderNumber}")]
	[Authorize(Roles = "warehouseman")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetWarehousemanOrder([FromRoute] string orderNumber, CancellationToken token)
	{
		return Ok(await _mediator.Send(new WarehousemanOrderDetailsQuery(orderNumber), token));
	}

	/// <summary>
	/// Retrieves Order with his details intended for Manager Role.
	/// </summary>
	/// <param name="orderId">OrderID for particular Order.</param>
	/// <param name="token"></param>
	/// <returns>Order Detaails Response by Id intended for Manager user role</returns>
	[HttpGet("manager-orderdetails/{orderId}")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetManagerOrder([FromRoute] string orderId, CancellationToken token)
	{
		return Ok(await _mediator.Send(new ManagerOrderDetailsQuery(orderId), token));
	}

	/// <summary>
	/// Update Order Status - Awaiting to Dispatch
	/// </summary>
	/// <param name="orderNumber">OrderNumber for particular Order.</param>
	/// <param name="token"></param>
	[HttpPut("awaiting-dispatch/{orderNumber}")]
	[Authorize(Roles = "warehouseman")]
	[EnableRateLimiting("limit-by-user-command")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> OrderAwaitingToDispach([FromRoute] string orderNumber, CancellationToken token)
	{
		await _mediator.Send(new AwaitingToDispatchCommand(orderNumber), token);
		return NoContent();
	}

	/// <summary>
	/// Update Order Status - Approve
	/// </summary>
	/// <param name="orderId">OrderNumber for particular Order.</param>
	/// <param name="token"></param>
	[HttpPut("approve/{orderId}")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-command")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> OrderApprove([FromRoute] string orderId, CancellationToken token)
	{
		await _mediator.Send(new ApproveCommand(orderId), token);
		return NoContent();
	}

	/// <summary>
	/// Update Order Status - Cancel
	/// </summary>
	/// <param name="command">Cancle Order Command Model.</param>
	/// <param name="token"></param>
	[HttpPut("cancel")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-command")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> OrderCancle([FromBody] CancelOrderCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}

	/// <summary>
	/// Retrieves Dynamic List with data for Buyer and Companies intended to create separate selected lists.
	/// </summary>
	/// <param name="token"></param>
	/// <returns>Selected Lists for Buyer and it's companies</returns>
	[HttpGet("buyer-selected-list")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrderBuyerSelectedList(CancellationToken token)
	{
		return Ok(await _mediator.Send(new OrderBuyerSelectedListQuery(), token));
	}

	/// <summary>
	/// Return Order Invoice PDF File Stream from Azure Blob if payment is full by paymentNumber or if 
	/// for particular invoice of that payment.
	/// </summary>
	/// <param name="paymentNumber"></param>
	/// <param name="installmentNumber"></param>
	/// <param name="token"></param>
	/// <returns>Order Invoice PDF File Response</returns>
	[HttpGet("payment/invoice")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrderInvoiceFile([FromQuery] string paymentNumber, [FromQuery] string? installmentNumber, CancellationToken token)
	{
		var query = new GetOrderInvoicePDFQuery(paymentNumber, installmentNumber);
		var response = await _mediator.Send(query);

		return File(response.FileStream, response.ContentType, response.FileName);
	}

	/// <summary>
	/// Retrieves data of Order Statistic
	/// </summary>
	/// <param name="token"></param>
	/// <returns>Order Statistic Response</returns>
	[HttpGet("statistics")]
	[Authorize(Roles = "manager")]
	[EnableRateLimiting("limit-by-user-query")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetOrderStatistics(CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetOrderStatisticsQuery(),token));
	}
}
