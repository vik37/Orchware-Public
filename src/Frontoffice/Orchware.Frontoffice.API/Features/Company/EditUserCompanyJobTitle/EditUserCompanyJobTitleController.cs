using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.EditUserJobTitle;

[Route("api/edit-user-company-job-title")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class EditUserCompanyJobTitleController : ControllerBase
{
	private readonly IMediator _mediator;

	public EditUserCompanyJobTitleController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Edit Company/User Job Title (Role).
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   Patch: api/edit-user-company-job-titl
	///   {
	///		  "jobTitle": "user role in the company (ex. Administrator, Manager, Owner, Head of Sales)"
	///   }
	/// </example>
	/// <param name="command">Filtering and pagination parameters</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="BadRequestException"></exception>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="EntityAlreadyExistsException"></exception>
	/// <exception cref="Exception"></exception>
	[HttpPatch]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateCompany([FromBody] EditUserCompanyJobTitleCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}
}
