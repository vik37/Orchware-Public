using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetAllFruitSeasons;

public record GetAllFruitSeasonsQuery : IRequest<object> { };

public class GetAllFruitSeasonsQueryHandler : IRequestHandler<GetAllFruitSeasonsQuery, object>
{
	public async Task<object> Handle(GetAllFruitSeasonsQuery request, CancellationToken cancellationToken)
	{
		var seasionalFruitsTask = Task.Run(() => Enum.GetValues(typeof(SeasonalFruits))
										.Cast<SeasonalFruits>()
										.Select(x => new
										{
											Index = (int)x,
											Name = x.GetDisplayNameToString(),
										}).ToList());

		return await seasionalFruitsTask;
	}
}
