using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
{
	private readonly DapperContext _dapperContext;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private readonly IUserContextService _userContextService;

	public GetOrdersHandler(DapperContext dapperContext, ISqlQueryBuilder sqlQueryBuilder, IUserContextService userContextService)
	{
		_dapperContext = dapperContext;
		_sqlQueryBuilder = sqlQueryBuilder;
		_userContextService = userContextService;
	}

	public async Task<GetOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetOrdersResponse();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		using var conn = _dapperContext.CreateConnection();

		var parameters = new DynamicParameters();
		parameters.Add("@UserId", _userContextService.Id, DbType.Guid);

		var orders = await conn.QueryAsync<OrderDto>(BuidSQL(), parameters);

		if(orders == null)
			return new GetOrdersResponse();

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

		return new GetOrdersResponse
		{
			Orders = orders.ToList(),
			TotalAmount = orders.Sum(o => o.TotalAmount),
			TotalCount = orders.Count()
		};
	}

	private string BuidSQL()
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
			.Where("NOT (o.[Status] = 'Cancelled' OR (o.[Status] = 'Approved' AND p.IsPaidInFull = 1))")
			.GroupBy(groupColumns)
			.OrderBy("o.OrderDate",false);

		return sqlBuilder.Build();
	}
}