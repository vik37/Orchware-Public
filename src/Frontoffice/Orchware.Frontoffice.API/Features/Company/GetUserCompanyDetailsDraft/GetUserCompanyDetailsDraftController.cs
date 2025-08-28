using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetailsDraft
{
	[Route("api/get-company/user-details/draft")]
	[ApiController]
	public class GetUserCompanyDetailsDraftController : ControllerBase
	{
		private readonly IMediator _mediator;

		public GetUserCompanyDetailsDraftController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Handles retrieving the company registration draft for the current logged-in user.
		/// The draft data is fetched from the cache.
		/// </summary>
		/// <example>
		///   Request:
		///   
		///   GET: api/get-company/user-details/draft
		///   
		/// </example>
		/// <param name="token">Cancellation token</param>
		/// <returns>User Profile including his Company Details</returns>
		[HttpGet]
		[Authorize(Policy = "NonEmployeeUser")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetUserCompanyDetails(CancellationToken token)
		{
			return Ok(await _mediator.Send(new GetUserCompanyDetailsDraftQuery(), token));
		}
	}
}
