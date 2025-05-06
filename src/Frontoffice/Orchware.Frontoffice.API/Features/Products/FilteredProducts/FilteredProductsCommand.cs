using MediatR;
using Orchware.Frontoffice.API.Common.Pagginations;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public record FilteredProductsCommand(RequestFilterPaggination Filter) 
	: IRequest<ResponseFilterPaggination<List<FilteredProductsDto>>>{}
