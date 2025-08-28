using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.GetCompanyById;

[Route("api/get-company-by-id")]
[ApiController]
[EnableRateLimiting("slide-by-ip")]
public class GetCompanyByIdController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetCompanyByIdController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Gets Company Data by specific ID.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/get-company-by-id/{1}
	///	  
	/// 
	/// </example>
	/// <param name="id">Company ID</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="Exception"></exception>
	/// <returns>Returns Company by ID</returns>
	[HttpGet("{id}")]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCompanyById([FromRoute] int id, CancellationToken token)
	{
		return Ok(await _mediator.Send(new GetCompanyByIdQuery(id),token));
	}
}
