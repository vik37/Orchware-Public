using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;

namespace Orchware.Frontoffice.API.Features.Company.UpdateCompany;

[Route("api/update-company")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class UpdateCompanyController : ControllerBase
{

	private readonly IMediator _mediator;

	public UpdateCompanyController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Update Company/User Prodile.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   PUT: api/update-company
	///   {
	///       "companyId": 12,
	///		  "companyName": "Company-Name",
	///		  "jobTitle": "user role in the company (ex. Administrator, Manager, Owner, Head of Sales)",
	///		  "companyEmail": "example@company.com",
	///		  "companyAddress": "(street name) No.(street number)",
	///		  "companyPhoneNumber": "company phone without country prefix (ex. 72[mobile/home operrator]333333[personal num's])",
	///		  "companyCity": "Skopje",
	///		  "companyLocation": "Center"
	///   }
	/// </example>
	/// <param name="command">Filtering and pagination parameters</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="BadRequestException"></exception>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="EntityAlreadyExistsException"></exception>
	/// <exception cref="Exception"></exception>
	[HttpPut]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyCommand command, CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}
}
