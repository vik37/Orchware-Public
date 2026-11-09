using Orchware.Backoffice.Application.Features.Orders.Shared;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Application.Features.PaymentSimulation.Queries;

public class GetPaymentInfoForOrderResponse
{
	public string OrderNumber { get; set; } = string.Empty;
	private string Status { get; set; } = string.Empty;
	public OrderStatusUIDto OrderStat { get => MapStatus(); }
	public DateTime OrderedDate { get; set; }
	public string Buyer_CompanyName { get; set; } = string.Empty;
	public string Buyer_PersonalEmail { get; set; } = string.Empty;
	public bool WillBePaidInFull { get; set; }
	public string PaymentNumber { get; set; } = string.Empty;
	public bool IsPaidInFull { get; set; }
	public DateTime PaidInFullDate { get; set; }
	public DateTime PaymentDate { get; set; }
	public bool HasInvoice { get; set; }

	public List<InstallmentsDto>? Installments { get; set; }

	private OrderStatusUIDto MapStatus()
	{
		var status = OrderStatus.GetAllStatuses().FirstOrDefault(x => x.Value == this.Status);

		if (status == null)
			throw new InvalidOperationException($"Order: {this.OrderNumber} has an invalid status '{Status}' in the database. Check data consistency.");

		return new OrderStatusUIDto
		{
			Value = status.Value,
			Color = status.Color
		};
	}
}

public class InstallmentsDto
{
	public string Code { get; set; } = string.Empty;
	public bool IsPaid { get; set; }
	public DateTime PaidOnDate { get; set; }
	public DateTime DueDate { get; set; }
	public bool HasInvoice { get; set; }
}
