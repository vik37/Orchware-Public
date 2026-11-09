using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Orders.Shared;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using Utilities.Pagination;

namespace Orchware.Backoffice.Application.Features.Orders.Commands.FilterHistoryOrder;

public record FilterHistoryOrderCommand(RequestFilterPagination Filter) : IRequest<FilterHistoryOrderResponse>;

public class FilterHistoryOrderCommandHandler : IRequestHandler<FilterHistoryOrderCommand, FilterHistoryOrderResponse>
{
	private readonly IUserContextService _userContextService;
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Order}]";

	public FilterHistoryOrderCommandHandler(IUserContextService userContextService,
		IOptions<DatabaseConnectionOption> dbConnection, ISqlQueryBuilder sqlQueryBuilder)
	{
		_userContextService = userContextService;
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
	}

	public async Task<FilterHistoryOrderResponse> Handle(FilterHistoryOrderCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new FilterHistoryOrderResponse();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		using var connection = new SqlConnection(_connectionString);

		var sql = BuildHistorySQL(request.Filter, out var parameters);
		var orders = (await connection.QueryAsync<OrderDto>(sql, parameters)).ToList();

		foreach (var order in orders)
		{
			if (order.Status == "Cancelled")
			{
				order.Status = "Failed";
				order.OrderStatus.Value = "Failed";
				order.OrderStatus.Color = "red";
			}
			else if (order.Status == "Approved")
			{
				order.Status = "Success";
				order.OrderStatus.Value = "Success";
				order.OrderStatus.Color = "green";
			}
		}

		var countSql = BuildHistoryCountSQL(request.Filter, out var countParameters);
		var totalCount = await connection.ExecuteScalarAsync<int>(countSql, countParameters);

		var (totalFailed, totalApproved) = await GetHistoryCountersAsync(connection, parameters);

		return new FilterHistoryOrderResponse
		{
			PageIndex = request.Filter.PageIndex,
			PageSize = request.Filter.PageSize,
			TotalCount = totalCount,
			TotalFailed = totalFailed,
			TotalApproved = totalApproved,
			Data = orders
		};
	}

	private string BuildHistorySQL(RequestFilterPagination filter, out DynamicParameters parameters)
	{
		parameters = new DynamicParameters();

		var selectColumns = @"o.Id as OrderId,
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
                        SUM(od.TotalPrice) - (SUM(od.TotalPrice) * o.Discount / 100.0) as TotalAmount,
                        SUM(
                            CASE od.UnitOfMeasure
                                WHEN 't' THEN od.Quantity * 1000.0  
                                WHEN 'g' THEN od.Quantity * 0.001
                                WHEN 'kg' THEN od.Quantity * 1.0
                                ELSE 0.0
                            END
                        ) AS TotalQuantityInKg";

		var groupColumns = "o.Id, o.OrderNumber, o.Buyer_Name, o.Buyer_CompanyName, o.OrderedDate, o.Discount, o.[Status], p.WillBePaidInFull, p.PaymentDate, p.FinalInstallmentDueDate";

		var sqlBuilder = _sqlQueryBuilder
			.Select(selectColumns)
			.From($"{Schema}.[Order] o")
			.Join("INNER", $"{Schema}.[OrderDetails] od", "od.OrderId = o.Id")
			.Join("INNER", $"{Schema}.[PaymentInfo] p", "p.OrderId = o.Id");

		sqlBuilder.Where("((o.[Status] = 'Cancelled' AND o.ModifiedDate < CAST(GETDATE() AS DATE)) OR (o.[Status] = 'Approved' AND p.IsPaidInFull = 1))");

		if (!string.IsNullOrEmpty(filter.Search))
		{
			sqlBuilder.Where("(o.OrderNumber LIKE @Search)");
			parameters.Add("@Search", $"%{filter.Search}%");
		}

		if(filter.MultyFilter != null && filter.MultyFilter.Count > 0)
			sqlBuilder = FilterStringBuilder(filter.MultyFilter, sqlBuilder);

		var orderByColumn = string.IsNullOrEmpty(filter.OrderBy) ? "o.OrderedDate" : filter.OrderBy;
		var isAscending = string.IsNullOrEmpty(filter.OrderBy) ? false : filter.OrderDirection?.ToUpper() == "ASC";

		return sqlBuilder.GroupBy(groupColumns)
							.OrderBy(orderByColumn, isAscending)
							.SkipTake(filter.PageIndex, filter.PageSize)
							.Build();
	}

	private ISqlQueryBuilder FilterStringBuilder(List<FilterKeyValue> filters, ISqlQueryBuilder sqlQueryBuilder)
	{
		foreach (var filter in filters)
		{
			if (filter.Key.Contains("Status"))
			{
				sqlQueryBuilder.Where($"([Status] = '{filter.Values.FirstOrDefault()}')");
			}

			if (filter.Key.Contains("from"))
			{
				sqlQueryBuilder.Where($"([OrderedDate] >= '{filter.Values.FirstOrDefault()}')");
			}

			if (filter.Key.Contains($"to"))
			{
				sqlQueryBuilder.Where($"([OrderedDate] <= '{filter.Values.FirstOrDefault()}')");
			}

			if (filter.Key.Contains("Buyer_Name"))
			{
				sqlQueryBuilder.Where($"([Buyer_Name] = '{filter.Values.FirstOrDefault()}')");
			}

			if (filter.Key.Contains($"Buyer_CompanyName"))
			{
				sqlQueryBuilder.Where($"([Buyer_CompanyName] = '{filter.Values.FirstOrDefault()}')");
			}
		}

		return sqlQueryBuilder;
	} 

	private string BuildHistoryCountSQL(RequestFilterPagination filter, out DynamicParameters parameters)
	{
		parameters = new DynamicParameters();

		var sqlBuilder = _sqlQueryBuilder
			.Select("COUNT(DISTINCT o.Id)")
			.From($"{Schema}.[Order] o")
			.Join("INNER", $"{Schema}.[OrderDetails] od", "od.OrderId = o.Id")
			.Join("INNER", $"{Schema}.[PaymentInfo] p", "p.OrderId = o.Id");

		sqlBuilder.Where("((o.[Status] = 'Cancelled' AND o.ModifiedDate < CAST(GETDATE() AS DATE)) OR (o.[Status] = 'Approved' AND p.IsPaidInFull = 1))");

		if (filter.MultyFilter != null && filter.MultyFilter.Count > 0)
			sqlBuilder = FilterStringBuilder(filter.MultyFilter, sqlBuilder);

		if (!string.IsNullOrEmpty(filter.Search))
		{
			sqlBuilder.Where("(o.OrderNumber LIKE @Search)");
			parameters.Add("@Search", $"%{filter.Search}%");
		}

		return sqlBuilder.Build();
	}

	private async Task<(int TotalFailed, int TotalApproved)> GetHistoryCountersAsync(SqlConnection connection, DynamicParameters parameters)
	{
		var sql = @"
					SELECT
						COUNT(DISTINCT CASE
							WHEN o.[Status] = 'Cancelled'
								 AND o.ModifiedDate < CAST(GETDATE() AS DATE)
							THEN o.Id
						END) AS TotalFailed,

						COUNT(DISTINCT CASE
							WHEN o.[Status] = 'Approved'
								 AND p.IsPaidInFull = 1
							THEN o.Id
						END) AS TotalApproved

					FROM " + Schema + @".[Order] o
					INNER JOIN " + Schema + @".[OrderDetails] od
						ON od.OrderId = o.Id
					INNER JOIN " + Schema + @".[PaymentInfo] p
						ON p.OrderId = o.Id

					WHERE
					(
						(o.[Status] = 'Cancelled'
							AND o.ModifiedDate < CAST(GETDATE() AS DATE))
						OR
						(o.[Status] = 'Approved'
							AND p.IsPaidInFull = 1)
					)";
		var result = await connection.QueryFirstOrDefaultAsync(sql, parameters);

		if (result == null)
			return (0, 0);

		return ((int)(result.TotalFailed ?? 0), (int)(result.TotalApproved ?? 0));
	}
}
