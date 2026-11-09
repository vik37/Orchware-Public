using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Backoffice.Application.Features.PaymentSimulation.Commands;
using Orchware.Backoffice.Application.Features.PaymentSimulation.Queries;

namespace Orchware.Backoffice.API.Controllers
{
	[Route("api/payment")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class PaymentController : ControllerBase
	{
		private readonly IMediator _mediator;

		public PaymentController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// List of Payment information about [in proccess] orders intended for payment simulation.
		/// </summary>
		/// <param name="token">Cancellation token.</param>
		/// <returns>No content if the order is successfully confirmed.</returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetPaymentInfo(CancellationToken token)
		{
			return Ok(await _mediator.Send(new GetPaymentInfoForOrderQuery()));
		}

		/// <summary>
		/// Maring Order that is paid (fully/per installment).
		/// Imitation of payment admin role clicks as if he is a banker and the client was about to pay. 
		/// The command is returned but in the background it checks with the company's budget (which is a random number created when creating a company) 
		/// if there is enough money from the amount of the order or the installment that was paid, then it is ok and it will be deducted from 
		/// the budget but if not then the manager is notified and an email is sent to the client
		/// </summary>
		/// <param name="command">OrderID for particular Order.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>No content if the order is successfully confirmed.</returns>
		[HttpPut]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[EnableRateLimiting("limit-by-user-command")]
		public async Task<IActionResult> MarkAsPaid([FromBody] MarkAsPaidCommand command, CancellationToken token)
		{
			await _mediator.Send(command, token);
			return NoContent();
		}
	}
}
