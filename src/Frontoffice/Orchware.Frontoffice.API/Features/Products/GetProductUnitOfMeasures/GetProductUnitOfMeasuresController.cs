using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Orchware.Frontoffice.API.Features.Products.GetProductUnitOfMeasures;

[Route("api/get-product-unit-of-measures")]
[ApiController]
public class GetProductUnitOfMeasuresController : ControllerBase
{
	private readonly IMediator _mediator;

	public GetProductUnitOfMeasuresController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Retrieves all unit of measures as an enumerable list.
	/// </summary>
	/// <example>
	///   GET aapi/get-product-unit-of-measures
	///   Response:
	///   [
	///      { "Index": 1, "Name": "g", "Fullname": "Grams" },
	///      { "Index": 2, "Name": "kg", "Fullname": "Kilograms" },
	///      { "Index": 3, "Name": "t", "Fullname": "Tones" }
	///   ]
	/// </example>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of unit of measures with display names and fullnames</returns>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		return Ok(await _mediator.Send(new GetProductUnitOfMeasuresQuery(), cancellationToken));
	}
}
