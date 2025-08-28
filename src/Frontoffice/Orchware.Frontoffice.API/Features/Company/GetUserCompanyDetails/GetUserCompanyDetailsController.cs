using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetails;

[Route("api/get-company")]
[ApiController]
[EnableRateLimiting("slide-by-ip")]
public class GetUserCompanyDetailsController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetUserCompanyDetailsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Gets Current Logged User Data including his Company.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   GET: api/get-company/1/user-details
	///   
	/// </example>
	/// <param name="companyId">Cancellation token</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="NotFoundException"></exception>
	/// <returns>User Profile including his Company Details</returns>
	[HttpGet("{companyId}/user-details")]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetUserCompanyDetails(int companyId, CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetUserCompanyDetailsQuery(companyId), token));
	}
}
