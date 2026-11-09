using MediatR;
using Utilities.Pagination;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public record FilteredProductsCommand(RequestFilterPagination Filter) 
	: IRequest<ResponsePagination<List<FilteredProductsDto>>>{}
