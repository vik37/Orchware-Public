using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetails;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistration;

[Route("api/company-registration")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class CompanyRegistrationController : ControllerBase
{
	private readonly IMediator _mediator;

	public CompanyRegistrationController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// User register new Company.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   POST: api/company-registration
	///   {
	///		  "companyName": "Company-Name",
	///		  "jobTitle": "user role in the company (ex. Administrator, Manager, Owner, Head of Sales)",
	///		  "companyEmail": "example@company.com",
	///		  "companyAddress": "(street name) No.(street number)",
	///		  "phoneNumber": "company phone without country prefix (ex. 72[mobile/home operrator]333333[personal num's])",
	///		  "companyCity": "Skopje",
	///		  "companyLocation": "Center"
	///   }
	/// </example>
	/// <param name="command">Filtering and pagination parameters</param>
	/// <param name="token">Cancellation token</param>
	/// <exception cref="BadRequestException"></exception>
	/// <exception cref="ForbiddenException"></exception>
	/// <exception cref="EntityAlreadyExistsException"></exception>
	/// <returns>Description with link - Location via header that points to the newly created resource.</returns>
	[HttpPost]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> RegisterNewCompany([FromBody] CompanyRegistrationCommand command,
		CancellationToken token)
	{
		var result = await _mediator.Send(command, token);

		return CreatedAtAction(actionName: $"{nameof(GetUserCompanyDetails)}", 
			controllerName: nameof(GetUserCompanyDetailsController).Replace("Controller",string.Empty), 
			routeValues: new { companyId = result }, null);
	}
}
