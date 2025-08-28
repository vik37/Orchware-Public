using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Features.Products.GetProductSeasonalFruits;

[Route("api/get-product-seasonal-fruits")]
[ApiController]
public class GetProductSeasonalFruitsController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetProductSeasonalFruitsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Retrieves all seasonal fruits as an enumerable list.
	/// </summary>
	/// <example>
	///   GET api/get-product-seasonal-fruits
	///   Response:
	///   [
	///      { "Index": 1, "Name": "Winter" },
	///      { "Index": 2, "Name": "Spring" },
	///      { "Index": 3, "Name": "Summer" }
	///   ]
	/// </example>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of seasonal fruits with display names</returns>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		return Ok(await _mediator.Send(new GetProductSeasonalFruitsQuery(),cancellationToken));
	}
}
