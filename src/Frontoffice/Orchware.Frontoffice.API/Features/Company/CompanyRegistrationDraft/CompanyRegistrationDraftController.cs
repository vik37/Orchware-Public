using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistrationDraft;

[Route("api/company-registration-draft")]
[ApiController]
public class CompanyRegistrationDraftController : ControllerBase
{
	private readonly IMediator _mediator;

	public CompanyRegistrationDraftController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Handles the saving of a company registration draft for the current logged-in user.
	/// The draft is stored temporarily and set to expire after a certain period.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   POST: api/ccompany-registration-draft
	///   {
	///		  "name": "Company-Name",
	///		  "jobTitle": "user role in the company (ex. Administrator, Manager, Owner, Head of Sales)",
	///		  "companyEmail": "example@company.com",
	///		  "address": "(street name) No.(street number)",
	///		  "phoneNumber": "company phone without country prefix (ex. 72[mobile/home operrator]333333[personal num's])",
	///		  "companyCity": "Skopje",
	///		  "companyLocation": "Center"
	///   }
	/// </example>
	/// <param name="command">Filtering and pagination parameters</param>
	/// <param name="token">Cancellation token</param>
	[HttpPost]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> RegisterNewCompany([FromBody] CompanyRegistrationDraftCommand command,
		CancellationToken token)
	{
		await _mediator.Send(command, token);
		return NoContent();
	}
}
