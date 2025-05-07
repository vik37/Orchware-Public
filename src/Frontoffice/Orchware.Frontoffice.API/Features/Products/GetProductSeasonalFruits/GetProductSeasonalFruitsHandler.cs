using MediatR;
using Orchware.Frontoffice.API.Features.Products.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Products.GetProductSeasonalFruits;

public class GetProductSeasonalFruitsHandler : IRequestHandler<GetProductSeasonalFruitsQuery,object>
{
	public async Task<object> Handle(GetProductSeasonalFruitsQuery request, CancellationToken cancellationToken)
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
