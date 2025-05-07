using MediatR;
using Orchware.Frontoffice.API.Features.Products.Shared.Enums;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Products.GetProductUnitOfMeasures;

public class GetProductUnitOfMeasuresHandler : IRequestHandler<GetProductUnitOfMeasuresQuery, 
																List<ProductUnitOfMeasuresDto>>
{
	public async Task<List<ProductUnitOfMeasuresDto>> Handle(GetProductUnitOfMeasuresQuery request, CancellationToken cancellationToken)
	{
		var unitsTask = Task.Run(() => Enum.GetValues(typeof(UnitsOfMeasure))
											.Cast<UnitsOfMeasure>()
											.Select(x => new ProductUnitOfMeasuresDto
											{
												Index = (int)x,
												Name = x.GetDisplayNameToString(),
												Fullname = x.ToString(),
											}).ToList());

		return await unitsTask;
	}
}
