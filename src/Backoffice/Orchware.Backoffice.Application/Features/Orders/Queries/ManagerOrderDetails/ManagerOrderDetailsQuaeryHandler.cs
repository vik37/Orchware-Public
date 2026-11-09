using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Application.Features.Shared.Enums;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Domain.Constants;
using System.Data;
using Utilities.Converters;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.ManagerOrderDetails;

public record ManagerOrderDetailsQuery(string OrderId) : IRequest<ManagerOrderDetailsResponse>;

public class ManagerOrderDetailsQuaeryHandler : IRequestHandler<ManagerOrderDetailsQuery, ManagerOrderDetailsResponse>
{
	private readonly string _connectionString;
	private string Schema = $"[{DomainSchema.Order}]";

	private readonly IUserContextService _userContextService;

	public ManagerOrderDetailsQuaeryHandler(IOptions<DatabaseConnectionOption> dbConnection,
		IUserContextService userContextService)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_userContextService = userContextService;
	}

	public async Task<ManagerOrderDetailsResponse> Handle(ManagerOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new ManagerOrderDetailsResponse();

		bool isParsed = Guid.TryParse(request.OrderId, out Guid result);

		if (!isParsed)
			throw new BadRequestException("Invalid type of Order ID");

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var parameters = new DynamicParameters();
								parameters.Add("@OrderId", result);
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
			await multi.ReadSingleOrDefaultAsync<ManagerOrderDetailsSqlDto>();

		if (header is null)
			throw new NotFoundException("Order not found");

		/* ======================================================
		   ITEMS
		====================================================== */

		var items =
			(await multi.ReadAsync<ManagerOrderItemSqlDto>())
			.ToList();

		/* ======================================================
		   PAYMENT
		====================================================== */

		var payment =
			await multi.ReadSingleOrDefaultAsync<PaymentDto>();

		/* ======================================================
		   MAP RESPONSE
		====================================================== */

		var response = new ManagerOrderDetailsResponse
		{
			OrderNumber = header.OrderNumber,
			OrderedDate = header.OrderedDate,
			Discount = header.Discount,
			Amount = header.Amount,
			TotalAmount = header.TotalAmount,
			TotalQuantityInKg = header.TotalQuantityInKg,
			Status = header.Status,
			CancellationReason = header.CancellationReason,
			CancelledBy = header.CancelledBy.HasValue ? ((CancelledBy)header.CancelledBy.Value).GetDisplayNameToString() : null,
			Buyer = new BuyerDto
			{
				PersonPlacedOrderName =
					header.PersonPlacedOrderName,

				PersonPlacedOrderEmailContact =
					header.PersonPlacedOrderEmailContact,

				CompanyName =
					header.CompanyName,

				CompanyAddress =
					header.CompanyAddress,

				CompanyEmail =
					header.CompanyEmail,

				CompanyCity =
					header.CompanyCity,

				CompanyLocation =
					header.CompanyLocation,

				CompanyPhoneNumber =
					header.CompanyPhoneNumber
			},
			Payment = new PaymentDto
			{
				IsFullySettled = payment.IsFullySettled,
				Deadline = payment.Deadline,
				NextIncomingPaymentDate = payment.NextIncomingPaymentDate,
				HasOverdueInstallments = payment.HasOverdueInstallments,
				WillBePaidInFull = payment.WillBePaidInFull,
				OverdueBalance = payment.OverdueBalance,
				HasPaymentInvoice = payment.HasPaymentInvoice,
				PaymentNumber = payment.PaymentNumber,
				TotalAmount = payment.TotalAmount,
				TotalRemainingAmount = payment.TotalRemainingAmount,
				IsFirstInstallmentPaid = payment.IsFirstInstallmentPaid,
				InstallmentAmount = payment.InstallmentAmount,
				PaidInstallmentsStatus = payment.PaidInstallmentsStatus,
				TotalInstallmentsCount = payment.TotalInstallmentsCount,
				InstallmentsWithInvoiceCount = payment.InstallmentsWithInvoiceCount
			},
			Items = items.Select(x => new OrderItemsDto
			{
				ShelfCode = x.ShelfCode,
				Quantity = x.Quantity,
				UnitOfMeasure = x.UnitOfMeasure,
				PricePerUnit = x.PricePerUnit,
				TotalPrice = x.TotalPrice,

				Product = new OrderedProductSnapshotDto
				{
					Name = x.ProductName,
					Image = x.ProductImage,
					SeasonalFruits = x.ProductSeasonalFruits,
					Price = x.ProductPrice,
					Units = x.ProductUnits
				}

			}).ToList()
		};

		return response;
	}
}
