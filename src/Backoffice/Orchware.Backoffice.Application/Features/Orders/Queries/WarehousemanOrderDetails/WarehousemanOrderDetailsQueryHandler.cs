using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using System.Data;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.WarehousemanOrderDetails;

public record WarehousemanOrderDetailsQuery(string OrderNumber) : IRequest<WarehousemanOrderDetailsResponse>;

public class WarehousemanOrderDetailsQueryHandler : IRequestHandler<WarehousemanOrderDetailsQuery, WarehousemanOrderDetailsResponse>
{
	private readonly string _connectionString;
	private string Schema = $"[{DomainSchema.Order}]";

	private readonly IUserContextService _userContextService;

	public WarehousemanOrderDetailsQueryHandler(IOptions<DatabaseConnectionOption> dbConnection, IUserContextService userContextService)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_userContextService = userContextService;
	}

	public async Task<WarehousemanOrderDetailsResponse> Handle(WarehousemanOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new WarehousemanOrderDetailsResponse();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var parameters = new DynamicParameters();
								parameters.Add("@OrderNumber", request.OrderNumber);
								parameters.Add("@UserRole", _userContextService.Role);

		using var connection = new SqlConnection(_connectionString);

		using var multi = await connection.QueryMultipleAsync(
											$"{Schema}.[SP_GetOrderDetails]",
											parameters,
											commandType: CommandType.StoredProcedure);

		/* ======================================================
		   HEADER
		====================================================== */

		var header =
			await multi.ReadSingleOrDefaultAsync<WarehousemanOrderDetailsSqlDto>();

		if (header is null)
			throw new NotFoundException("Order not found");

		/* ======================================================
		   ITEMS
		====================================================== */

		var items =
			(await multi.ReadAsync<WarehousemanOrderItemSqlDto>())
			.ToList();

		/* ======================================================
		   MAP RESPONSE
		====================================================== */

		var response = new WarehousemanOrderDetailsResponse
		{
			OrderNumber = header.OrderNumber,
			OrderedDate = header.OrderedDate,
			TotalQuantityInKg = header.TotalQuantityInKg,
			Status = header.Status,

			BuyerCompanyName = header.CompanyName,
			BuyerCompanyAddress = header.CompanyAddress,
			BuyerCompanyCity = header.CompanyCity,
			BuyerCompanyLocation = header.CompanyLocation,
			BuyerCompanyPhoneNumber = header.CompanyPhoneNumber,
			
			Items = items.Select(x => new OrderItemsDto
			{
				ShelfCode = x.ShelfCode,
				Quantity = x.Quantity,
				UnitOfMeasure = x.UnitOfMeasure,

				Product = new OrderedProductPreviewDto
				{
					Name = x.ProductName,
					Image = x.ProductImage
				}

			}).ToList()
		};

		return response;
	}
}
