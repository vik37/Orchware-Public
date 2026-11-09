using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Filters;

namespace Orchware.Frontoffice.API.Features.Orders.DeletePendingOrder;

[Route("api/delete-order-pending")]
[Authorize(Policy = "NonEmployeeUser")]
[ApiController]
public class DeletePendingOrderController : ControllerBase
{
	private readonly IMediator _mediator;

	public DeletePendingOrderController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Delete pending (pre-confirmed) order detail for the logged-in user:
	/// If tempId = all it will clean whole order made this user,
	/// If tempId = b14b6e2e-0d2f-4195-8e54-0c053b04afd2 (Guid) order detail contains this tempId will be removed.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   DELETE: api/delete-order-pending?tempId=all
	///   
	/// </example>
	/// <param name="tempId">Order Pending Temporary ID</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="BadRequestException"></exception>
	/// <returns>No Content</returns>
	[HttpDelete]
	[ServiceFilter(typeof(RequireCompanyFilter))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DeleteOrderDetail([FromQuery] string tempId, CancellationToken token)
	{
		await _mediator.Send(new DeletePendingOrderCommand(tempId), token);
		return NoContent();
	}
}
