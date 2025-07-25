using AutoMapper;
using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Pagginations;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public class FilteredProductsHandler : IRequestHandler<
	FilteredProductsCommand,
	ResponseFilterPaggination<List<FilteredProductsDto>>>
{
	private readonly DapperContext _dapperContext;
	private readonly IMapper _mapper;

	public FilteredProductsHandler(DapperContext dapperContext, IMapper mapper)
	{
		_dapperContext = dapperContext;
		_mapper = mapper;
	}

	public async Task<ResponseFilterPaggination<List<FilteredProductsDto>>> Handle(FilteredProductsCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return new ResponseFilterPaggination<List<FilteredProductsDto>>
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

		return new ResponseFilterPaggination<List<FilteredProductsDto>>
		{
			PageIndex = request.Filter.PageIndex,
			PageSize = request.Filter.PageSize,
			TotalCount = total,
			Data = _mapper.Map<List<FilteredProductsDto>>(items)
		};
	}
}
