using Dapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrderDetails;

public class GetOrderDetailsHandler : IRequestHandler<GetOrderDetailsQuery, GetOrderDetailsResponse>
{
	private readonly DapperContext _dapperContext;
	private readonly IUserContextService _userContextService;

	public GetOrderDetailsHandler(DapperContext dapperContext, 
		IUserContextService userContext)
	{
		_dapperContext = dapperContext;
		_userContextService = userContext;
	}

	public async Task<GetOrderDetailsResponse> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetOrderDetailsResponse();

		bool isParsed = Guid.TryParse(request.orderId, out Guid result);

		if (!isParsed)
			throw new BadRequestException("Invalid type of Order ID");

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var parameters = new DynamicParameters();
			parameters.Add("@OrderId", result);
			parameters.Add("@UserId", _userContextService.Id);

		using var multi = await _dapperContext.ExecuteMultipleStoredProcedureAsync("[dbo].[GetOrderDetails]", parameters);

		var response = await multi.ReadSingleOrDefaultAsync<GetOrderDetailsResponse>();

		if (response != null)
		{
			response.Items = (await multi.ReadAsync<OrderItemDto>()).ToList();

			var paymentRows = (await multi.ReadAsync<dynamic>()).ToList();

			if (paymentRows.Any())
			{
				var first = paymentRows.First();
				response.PaymentInfo = new PaymentDto
				{
					WillBePaidInFull = first.WillBePaidInFull,
					NumberOfInstallments = first.NumberOfInstallments,
					AmountPerInstallment = first.AmountPerInstallment,
					PaidInFullDate = first.PaidInFullDate,

					Installments = paymentRows
						.Where(r => r.DueDate != null)
						.Select(r => new InstallmentDto
						{
							DueDate = r.DueDate,
							Amount = r.Amount,
							IsPaid = r.IsPaid,
							PaidOnDate = r.PaidOnDate
						}).ToList()
				};
			}
		}

		return response;
	}
}
