using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using System.Data;
using Utilities.Pagination;

namespace Orchware.Frontoffice.API.Features.Orders.FilterHistoryOrder;

public class FilterHistoryOrderHandler : IRequestHandler<FilterHistoryOrderCommand, ResponsePagination<List<OrderDto>>>
{
	private readonly DapperContext _dapperContext;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private readonly IUserContextService _userContextService;

	public FilterHistoryOrderHandler(DapperContext dapperContext,
		ISqlQueryBuilder sqlQueryBuilder, IUserContextService userContextService)
	{
		_dapperContext = dapperContext;
		_sqlQueryBuilder = sqlQueryBuilder;
		_userContextService = userContextService;
	}

	public async Task<ResponsePagination<List<OrderDto>>> Handle(FilterHistoryOrderCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return new ResponsePagination<List<OrderDto>>
			{
				PageIndex = request.Filter.PageIndex + 1,
				PageSize = request.Filter.PageSize,
				TotalCount = 0,
				Data = new List<OrderDto>()
			};
		}

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		using var conn = _dapperContext.CreateConnection();

		var parameters = new DynamicParameters();
		parameters.Add("@UserId", _userContextService.Id, DbType.Guid);

		var query = BuidSQL(request.Filter, parameters);

		var orders = await conn.QueryAsync<OrderDto>(query, parameters);

		var total = await GetTotalCountAsync(conn, request.Filter, parameters);

		var statusMap = OrderStatus.GetAllStatuses().ToDictionary(s => s.Value, s => s);

		foreach (var order in orders)
		{
			if (statusMap.TryGetValue(order.Status, out var domainStatus))
			{
				order.OrderStatus.Value = domainStatus.Value;
				order.OrderStatus.Color = domainStatus.Color;
			}
			else
			{
				throw new InvalidOperationException($"Order ID: {order.OrderId} has an invalid status '{order.Status}' in the database. Check data consistency.");
			}
		}

		if (orders == null)
		{
			return new ResponsePagination<List<OrderDto>>
			{
				PageIndex = request.Filter.PageIndex + 1,
				PageSize = request.Filter.PageSize,
				TotalCount = 0,
				Data = new List<OrderDto>()
			};
		}

		return new ResponsePagination<List<OrderDto>>
		{
			PageIndex = request.Filter.PageIndex,
			PageSize = request.Filter.PageSize,
			TotalCount = total,
			Data = orders.ToList()
		};
	}

	private string BuidSQL(FilterHistoryOrderRequest filter, DynamicParameters parameters)
	{
		var selectColumns = @"o.Id as OrderId,
                      o.OrderNumber,
                      o.OrderDate,
                      o.Discount,
                      o.[Status],
                      SUM(od.TotalPrice) as Amount,
                      SUM(od.TotalPrice) - (SUM(od.TotalPrice) * o.Discount / 100.0) as TotalAmount,
                      SUM(
                          CASE od.UnitOfMeasure
                              WHEN 't' THEN od.Quantity * 1000.0  
                              WHEN 'g' THEN od.Quantity * 0.001
                              WHEN 'kg' THEN od.Quantity * 1.0
                              ELSE 0.0
                          END
                      ) AS TotalQuantityInKg";

		var groupColumns = "o.Id, o.OrderNumber, o.OrderDate, o.Discount, o.[Status]";

		var sqlBuilder = _sqlQueryBuilder
			.Select(selectColumns)
			.From("[dbo].[Order] o")
			.Join("INNER", "[dbo].[OrderDetails] od", "od.OrderId = o.Id")
			.Join("INNER", "[dbo].[Payment] p", "p.OrderId = o.Id")
			.Where("o.UserId = @UserId")
			.Where("(o.[Status] = 'Cancelled' OR (o.[Status] = 'Approved' AND p.IsPaidInFull = 1))");

		if (!string.IsNullOrWhiteSpace(filter.OrderStatus))
		{
			sqlBuilder.Where("o.[Status] = @OrderStatus");
			parameters.Add("@OrderStatus", filter.OrderStatus);
		}

		if (filter.From.HasValue)
		{
			sqlBuilder.Where("o.OrderDate >= @FromDate");
			parameters.Add("@FromDate", filter.From.Value, DbType.DateTime);
		}

		if (filter.To.HasValue)
		{
			sqlBuilder.Where("o.OrderDate <= @ToDate");
			parameters.Add("@ToDate", filter.To.Value, DbType.DateTime);
		}

		int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
		int pageIndex = filter.PageIndex > 0 ? filter.PageIndex : 1;

		return sqlBuilder
			.GroupBy(groupColumns)
			.OrderBy("o.OrderDate", ascending: false)
			.SkipTake(pageIndex, pageSize)
			.Build();
	}

	private async Task<int> GetTotalCountAsync(IDbConnection conn, FilterHistoryOrderRequest filter, DynamicParameters parameters)
	{
		var sqlBuilder = _sqlQueryBuilder
			.Select("COUNT(DISTINCT o.Id)")
			.From("[dbo].[Order] o")
			.Join("INNER", "[dbo].[OrderDetails] od", "od.OrderId = o.Id")
			.Join("INNER", "[dbo].[Payment] p", "p.OrderId = o.Id")
			.Where("o.UserId = @UserId")
			.Where("(o.[Status] = 'Cancelled' OR (o.[Status] = 'Approved' AND p.IsPaidInFull = 1))");

		if (!string.IsNullOrWhiteSpace(filter.OrderStatus))
		{
			sqlBuilder.Where("o.[Status] = @OrderStatus");
		}

		if (filter.From.HasValue)
		{
			sqlBuilder.Where("o.OrderDate >= @FromDate");
		}

		if (filter.To.HasValue)
		{
			sqlBuilder.Where("o.OrderDate <= @ToDate");
		}

		var countSql = sqlBuilder.Build();

		return await conn.QuerySingleAsync<int>(countSql, parameters);
	}
}
