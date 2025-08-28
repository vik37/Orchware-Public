using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.HasRegisteredCompany;

[Route("api/has-registered-company")]
[ApiController]
[EnableRateLimiting("slide-by-ip")]
public class HasRegisteredCompanyController : ControllerBase
{
	private readonly IMediator _mediator;

	public HasRegisteredCompanyController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Check if current logged user has registered company and return response object.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/has-registered-company
	///	  
	/// 
	/// </example>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="Exception"></exception>
	/// <returns>Returns (true/false) dependse of logged user has registered company include companyId</returns>
	[HttpGet]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCompanyById(CancellationToken token)
	{
		return Ok(await _mediator.Send(new HasRegisteredCompanyQuery(), token));
	}
}
