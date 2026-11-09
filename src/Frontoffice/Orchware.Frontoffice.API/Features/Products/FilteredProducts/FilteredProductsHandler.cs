using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using Utilities.Converters;
using Utilities.Pagination;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public class FilteredProductsHandler : IRequestHandler<
	FilteredProductsCommand,
	ResponsePagination<List<FilteredProductsDto>>>
{
	private readonly DapperContext _dapperContext;

	public FilteredProductsHandler(DapperContext dapperContext)
	{
		_dapperContext = dapperContext;
	}

	public async Task<ResponsePagination<List<FilteredProductsDto>>> Handle(FilteredProductsCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return new ResponsePagination<List<FilteredProductsDto>>
			{
				PageIndex = request.Filter.PageIndex + 1,
				PageSize = request.Filter.PageSize,
				TotalCount = 0,
				Data = new List<FilteredProductsDto>()
			};
		}

		var parameters = new DynamicParameters();
		parameters.Add("@Page", request.Filter.PageIndex);
		parameters.Add("@PageSize", request.Filter.PageSize);
		parameters.Add("@Order", !string.IsNullOrEmpty(request.Filter.OrderBy) ? request.Filter.OrderBy : "Id");
		parameters.Add("@OrderDirection", !string.IsNullOrEmpty(request.Filter.OrderDirection) ? request.Filter.OrderDirection : "ASC");
		parameters.Add("@SearchTerm", request.Filter.Search ?? "");
		
		if(request.Filter.MultyFilter is not null)
		{
			string whereClause = SqlFilterHelper.BuildSafeWhereClause(request.Filter.MultyFilter);
			parameters.Add("@MultiFilter", whereClause);
		}

		var (total, items) = await _dapperContext.ExecutePagedProcedureAsync<Product>("ProductPaginationFilter", parameters);

		return new ResponsePagination<List<FilteredProductsDto>>
		{
			PageIndex = request.Filter.PageIndex,
			PageSize = request.Filter.PageSize,
			TotalCount = total,
			Data = items == null ? null : MapFromProductToFilterProductDto(items).ToList()
		};
	}

	private IEnumerable<FilteredProductsDto> MapFromProductToFilterProductDto(List<Product> products)
	{
		foreach (var product in products)
		{
			yield return new FilteredProductsDto
			{
				Id = product.Id,
				AvailableQuantity = product.AvailableQuantity,
				MinQuantity = product.MinQuantity,
				SeasonalFruits = product.SeasonalFruits.GetDisplayNameToString(),
				Name = product.Name,
				Price = product.Price,
				Units = product.Units.GetDisplayNameToString(),
				Image = product.Image
			};
		}
	}
}
