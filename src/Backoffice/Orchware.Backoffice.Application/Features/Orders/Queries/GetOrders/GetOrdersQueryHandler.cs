using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Orders.Shared;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using System.Data;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery(GetOrdersRequest Request) : IRequest<GetOrdersResponse>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
{
	private readonly IUserContextService _userContextService;
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;

	private string Schema = $"[{DomainSchema.Order}]";

	public GetOrdersQueryHandler(
		IUserContextService userContextService,
		IOptions<DatabaseConnectionOption> dbConnection,
		ISqlQueryBuilder sqlQueryBuilder)
	{
		_userContextService = userContextService;
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<GetOrdersResponse> Handle(
		GetOrdersQuery request,
		CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetOrdersResponse();

		if (_userContextService?.Id is null)
			throw new ForbiddenException(
				"You don't have permission to perform this action.");

		var statusMap = OrderStatus
			.GetAllStatuses()
			.ToDictionary(s => s.Value, s => s);

		if (string.IsNullOrEmpty(request.Request.Status) ||
			request.Request.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
		{
		}
		else if (!statusMap.ContainsKey(request.Request.Status))
		{
			throw new BadRequestException(
				$"Invalid Status '{request.Request.Status}'. " +
				$"Allowed values are: All, {string.Join(", ", statusMap.Keys)}");
		}

		var isManager = _userContextService!.Role!.Equals(
			VisibilityLevel.Manager.Value,
			StringComparison.OrdinalIgnoreCase);

		var isAllStatus =
			string.IsNullOrEmpty(request.Request.Status) ||
			request.Request.Status.Equals("All", StringComparison.OrdinalIgnoreCase);

		var canUsePagination = isManager && isAllStatus;

		using var connection = new SqlConnection(_connectionString);

		var parameters = new DynamicParameters();

		parameters.Add(
			"@Status",
			request.Request.Status,
			DbType.String);

		var summary = await connection.QuerySingleAsync<OrderSummary>(
			BuildCountAndTotalAmountSQL(request.Request.Status),
			parameters);

		var totalCount = summary.TotalCount;
		var usePagination = canUsePagination && totalCount > 10;

		var orders = await connection.QueryAsync<OrderDto>(
			BuildSQL(
				request.Request.Status,
				request.Request.pageIndex,
				request.Request.pageSize,
				usePagination),
			parameters);

		if (orders == null)
		{
			return new GetOrdersResponse
			{
				Orders = new List<OrderDto>(),
				TotalAmount = summary.TotalAmount,
				TotalCount = summary.TotalCount
			};
		}

		foreach (var order in orders)
		{
			if (statusMap.TryGetValue(order.Status, out var domainStatus))
			{
				order.OrderStatus.Value = domainStatus.Value;
				order.OrderStatus.Color = domainStatus.Color;
			}
			else
			{
				throw new InvalidOperationException(
					$"Order ID: {order.OrderId} has an invalid status " +
					$"'{order.Status}' in the database. Check data consistency.");
			}
		}

		var orderList = orders.ToList();

		return new GetOrdersResponse
		{
			Orders = orderList,
			TotalAmount = summary.TotalAmount,
			TotalCount = summary.TotalCount
		};
	}

	private string BuildCountAndTotalAmountSQL(string status)
	{
		var sqlBuilder = _sqlQueryBuilder
			.Select(@"
				o.Id AS OrderId,
				o.Discount,
				SUM(od.TotalPrice) -
					(SUM(od.TotalPrice) * o.Discount / 100.0) AS TotalAmount")
			.From($"{Schema}.[Order] o")
			.Join(
				"INNER",
				$"{Schema}.[OrderDetails] od",
				"od.OrderId = o.Id")
			.Join(
				"INNER",
				$"{Schema}.[PaymentInfo] p",
				"p.OrderId = o.Id");

		if (!string.IsNullOrEmpty(status) &&
			!status.Equals("All", StringComparison.OrdinalIgnoreCase))
		{
			sqlBuilder.Where("o.[Status] = @Status");
		}

		if (_userContextService!.Role!.Equals(
			VisibilityLevel.Warehouseman.Value,
			StringComparison.OrdinalIgnoreCase))
		{
			sqlBuilder
				.Where($"o.VisibilityLevel = '{VisibilityLevel.Warehouseman.Value}'")
				.Where(@"
					NOT (
						(o.[Status] = 'Cancelled'
							AND o.ModifiedDate < DATEADD(day, -3, CAST(GETDATE() AS DATE)))
						OR
						(o.[Status] = 'Approved' AND o.ModifiedDate < DATEADD(day, -3, CAST(GETDATE() AS DATE)))
					)");
		}
		else
		{
			sqlBuilder.Where(@"
				NOT (
					(o.[Status] = 'Cancelled'
						AND o.ModifiedDate < CAST(GETDATE() AS DATE))
					OR
					(o.[Status] = 'Approved' AND p.IsPaidInFull = 1)
				)");
		}

		sqlBuilder.GroupBy(@"
			o.Id,
			o.Discount");

		return $@"
			SELECT
				COUNT(*) AS TotalCount,
				COALESCE(SUM(TotalAmount), 0) AS TotalAmount
			FROM (
				{sqlBuilder.Build()}
			) AS OrderSummary";
	}

	private string BuildSQL(
		string status,
		int? pageIndex,
		int? pageSize,
		bool usePagination)
	{
		var selectColumns = @"
			o.Id as OrderId,
			o.OrderNumber,
			o.OrderedDate,
			o.Buyer_Name as Buyer,
			o.Buyer_CompanyName as Company,
			p.WillBePaidInFull,
			CASE 
				WHEN p.WillBePaidInFull = 1 THEN p.PaymentDate 
				ELSE p.FinalInstallmentDueDate 
			END AS Deadline,
			o.Discount,
			o.[Status],
			SUM(od.TotalPrice) as Amount,
			SUM(od.TotalPrice) -
				(SUM(od.TotalPrice) * o.Discount / 100.0) as TotalAmount,
			SUM(
				CASE od.UnitOfMeasure
					WHEN 't' THEN od.Quantity * 1000.0
					WHEN 'g' THEN od.Quantity * 0.001
					WHEN 'kg' THEN od.Quantity * 1.0
					ELSE 0.0
				END
			) AS TotalQuantityInKg";

		var groupColumns = @"
			o.Id,
			o.OrderNumber,
			o.Buyer_Name,
			o.Buyer_CompanyName,
			o.OrderedDate,
			o.Discount,
			o.[Status],
			p.WillBePaidInFull,
			p.PaymentDate,
			p.FinalInstallmentDueDate";

		var sqlBuilder = _sqlQueryBuilder
			.Select(selectColumns)
			.From($"{Schema}.[Order] o")
			.Join(
				"INNER",
				$"{Schema}.[OrderDetails] od",
				"od.OrderId = o.Id")
			.Join(
				"INNER",
				$"{Schema}.[PaymentInfo] p",
				"p.OrderId = o.Id");

		if (!string.IsNullOrEmpty(status) &&
			!status.Equals("All", StringComparison.OrdinalIgnoreCase))
		{
			sqlBuilder.Where("o.[Status] = @Status");
		}

		if (_userContextService!.Role!.Equals(
			VisibilityLevel.Warehouseman.Value,
			StringComparison.OrdinalIgnoreCase))
		{
			sqlBuilder
				.Where($"o.VisibilityLevel = '{VisibilityLevel.Warehouseman.Value}'")
				.Where(@"
					NOT (
						(o.[Status] = 'Cancelled'
							AND o.ModifiedDate < DATEADD(day, -3, CAST(GETDATE() AS DATE)))
						OR
						(o.[Status] = 'Approved' AND o.ModifiedDate < DATEADD(day, -3, CAST(GETDATE() AS DATE)))
					)");
		}
		else
		{
			sqlBuilder.Where(@"
				NOT (
					(o.[Status] = 'Cancelled'
						AND o.ModifiedDate < CAST(GETDATE() AS DATE))
					OR
					(o.[Status] = 'Approved' AND p.IsPaidInFull = 1)
				)");
		}

		sqlBuilder
			.GroupBy(groupColumns)
			.OrderBy("o.OrderedDate", false);

		if (usePagination)
		{
			sqlBuilder.SkipTake(
				pageIndex ?? 1,
				pageSize ?? 10);
		}

		return sqlBuilder.Build();
	}

	private class OrderSummary
	{
		public int TotalCount { get; set; }
		public decimal TotalAmount { get; set; }
	}
}