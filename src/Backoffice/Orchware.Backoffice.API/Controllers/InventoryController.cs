using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Orchware.Backoffice.Application.Features.Inventory.Queries.GetAllFruitSeasons;
using Orchware.Backoffice.Application.Features.Inventory.Queries.GetAllUnitsOfMeasure;
using Orchware.Backoffice.Application.Features.Inventory.Queries.GetShelfDescriptions;
using Orchware.Backoffice.Application.Features.Inventory.Queries.GetShelvesBySeason;
using Orchware.Backoffice.Application.Features.Inventory.Queries.GetSpecificShelfById;
using Orchware.Backoffice.Application.Features.Shared.Enums;

namespace Orchware.Backoffice.API.Controllers
{
	[Route("api/inventory")]
	[EnableRateLimiting("slide-by-ip")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly IMediator _mediator;

		public InventoryController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get Shelf Description including brief details of the products (name and quantity) 
		/// and total count of products.
		/// </summary>
		/// <example>
		///   Request:
		///   
		///   GET: api/inventory
		///   
		/// </example>
		/// <param name="token"></param>
		/// <returns>Get Shelf Description DTO, including a List of Shelves with their brief details in list of products</returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Get(CancellationToken token)
		{
			return Ok(await _mediator.Send(new GetShelfDescriptionQuery(),token));
		}

		/// <summary>
		/// Gets Shelves by Seasonal Fruit.
		/// </summary>
		/// <example>
		///   Request:
		///   
		///   GET: api/inventory/season/1 or winter
		///   
		/// </example>
		/// <param name="seasonalFruits"></param>
		/// <param name="token"></param>
		/// <returns>List of Shelves by Seasonal Fruits</returns>
		[HttpGet("season/{seasonalFruits}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Get([FromRoute] SeasonalFruits seasonalFruits, CancellationToken token)
		{
			return Ok(await _mediator.Send(new GetShelvesBySeasonQuery(seasonalFruits), token));
		}

		/// <summary>
		/// Get specific Shelf by Id including full details of the Product placed on this Shelf.
		/// </summary>
		/// <example>
		///   Request:
		///   
		///   GET: api/inventory/shelf/2
		///   
		/// </example>
		/// <param name="id"></param>
		/// <param name="token"></param>
		/// <returns>Shelf DTO, including including the product that is placed on the shelf.</returns>
		[HttpGet("shelf/{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSpecificShelfById([FromRoute] int id, CancellationToken token)
		{
			return Ok(await _mediator.Send(new GetSpecificShelfByIdQuery(id), token));
		}

		/// <summary>
		/// Retrieves all seasonal fruits as an enumerable list.
		/// </summary>
		/// <example>
		///   GET api/inventory/seasonal-fruits
		///   Response:
		///   [
		///      { "Index": 1, "Name": "Winter" },
		///      { "Index": 2, "Name": "Spring" },
		///      { "Index": 3, "Name": "Summer" }
		///   ]
		/// </example>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of seasonal fruits with display names</returns>
		[HttpGet("seasonal-fruits")]
		[DisableRateLimiting]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllFruitSeasons(CancellationToken cancellationToken)
		{
			return Ok(await _mediator.Send(new GetAllFruitSeasonsQuery(), cancellationToken));
		}

		/// <summary>
		/// Retrieves all unit of measures as an enumerable list.
		/// </summary>
		/// <example>
		///   GET api/inventory/units-of-measure
		///   Response:
		///   [
		///      { "Index": 1, "Name": "g", "Fullname": "Grams" },
		///      { "Index": 2, "Name": "kg", "Fullname": "Kilograms" },
		///      { "Index": 3, "Name": "t", "Fullname": "Tones" }
		///   ]
		/// </example>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of unit of measures with display names and fullnames</returns>
		[HttpGet("units-of-measure")]
		[DisableRateLimiting]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllUnitsOfMeasure(CancellationToken cancellationToken)
		{
			return Ok(await _mediator.Send(new GetAllUnitsOfMeasureQuery(), cancellationToken));
		}
	}
}
