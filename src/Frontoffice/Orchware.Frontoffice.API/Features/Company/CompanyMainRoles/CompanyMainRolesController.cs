using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.Constants;

namespace Orchware.Frontoffice.API.Features.Company.CompanyMainRoles;

[Route("api/company-main-roles")]
[ApiController]
public class CompanyMainRolesController : ControllerBase
{
	/// <summary>
	/// Gets list of main user/company job titles.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///	  GET: 	api/company-main-roles
	///	  
	/// </example>
	/// <returns>Returns List of User (Job Titles) Roles in Company.</returns>
	[HttpGet]
	[Authorize(Policy = "NonEmployeeUser")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public IActionResult GetCompanyRoles()
	{
		return Ok(new List<string> { UserCompanyDefaultRoles.CompanyOwner, UserCompanyDefaultRoles.CompanyAdmin, UserCompanyDefaultRoles.CompanyManager});
	}
}
