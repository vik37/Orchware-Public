using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Pagginations;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

[Route("api/products")]
[ApiController]
public class FilteredProductsController : ControllerBase
{
	private readonly IMediator _mediator;

	public FilteredProductsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Filters products based on the given parameters.
	/// </summary>
	/// <example>
	///   Request:
	///   
	///   api/products
	///   {
	///		  "pageIndex": 1,
	///		  "pageSize": 10
	///   }
	/// </example>
	/// <param name="requestFilter">Filtering and pagination parameters</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <exception cref="BadRequestException"></exception>
	/// <returns>Filtered list of products including description</returns>
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<FilteredProductsDto>>> FilteredProducts([FromBody] RequestFilterPaggination requestFilter,
		CancellationToken cancellationToken)
	{
		return Ok(await _mediator.Send(new FilteredProductsCommand(requestFilter), cancellationToken));
	}
}
