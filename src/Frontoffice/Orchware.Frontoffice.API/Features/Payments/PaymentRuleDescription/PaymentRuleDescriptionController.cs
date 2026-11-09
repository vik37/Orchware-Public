using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Features.Payments.PaymentRuleDescription;

[Route("api/payment-rule-description")]
[ApiController]
[Authorize(Policy = "NonEmployeeUser")]
public class PaymentRuleDescriptionController : ControllerBase
{
	private IMediator _mediator;

	public PaymentRuleDescriptionController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Get Rules for Payment Input Fields and Text Descriptions. 
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/payment-rule-description
	///	  
	/// 
	/// </example>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="Exception"></exception>
	/// <returns>Payment rules for Pending Order Details plus Text Descriptions</returns>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> PaymentRuleDescription(CancellationToken token)
	{
		return Ok(await _mediator.Send(new PaymentRuleDescriptionQuery()));
	}
}
