using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.AssignUserToCompany;

[Route("api/assign-user-to-company")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class AssignUserToCompanyController : ControllerBase
{
	private readonly IMediator _mediator;

	public AssignUserToCompanyController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Assign User to existing Company.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   POST: api/assign-user-to-company
	///   {
	///		  "companyId": 12,
	///		  "jobTitle": "user role in the company (ex. Administrator, Manager, Owner, Head of Sales)"
	///   }
	/// </example>
	/// <param name="command">Filtering and pagination parameters</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="BadRequestException"></exception>
	/// <exception cref="ForbiddenException"></exception>
	/// <returns>Proper message.</returns>
	[HttpPost]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> AssignUser([FromBody] AssignUserToCompanyCommand command, CancellationToken token)
	{
		return Ok(await _mediator.Send(command, token));
	}
}
