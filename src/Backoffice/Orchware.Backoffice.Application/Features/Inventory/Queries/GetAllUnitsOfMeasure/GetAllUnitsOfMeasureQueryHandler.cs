using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Inventory.Queries.GetAllUnitsOfMeasure;

public record GetAllUnitsOfMeasureQuery : IRequest<List<ProductUnitOfMeasuresResponse>> { }

public class GetAllUnitsOfMeasureQueryHandler : IRequestHandler<GetAllUnitsOfMeasureQuery, List<ProductUnitOfMeasuresResponse>>
{
	public async Task<List<ProductUnitOfMeasuresResponse>> Handle(GetAllUnitsOfMeasureQuery request, 
		CancellationToken cancellationToken)
	{
		var unitsTask = Task.Run(() => Enum.GetValues(typeof(UnitsOfMeasure))
											.Cast<UnitsOfMeasure>()
											.Select(x => new ProductUnitOfMeasuresResponse
											{
												Index = (int)x,
												Name = x.GetDisplayNameToString(),
												Fullname = x.ToString(),
											}).ToList());

		return await unitsTask;
	}
}
