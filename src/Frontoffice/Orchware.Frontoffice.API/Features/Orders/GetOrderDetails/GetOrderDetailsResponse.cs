using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;
using System.Text.Json.Serialization;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrderDetails;

public class GetOrderDetailsResponse
{
	public string OrderNumber { get; set; } = string.Empty;
	public OrderStatusUIDto StatusUI { get; set; } = new();
	[JsonIgnore]
	public string Status
	{
		set => MapStatus(value);
	}
	public DateTime OrderDate { get; set; }
	public decimal Discount { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string CompanyLocation { get; set; } = string.Empty;

	public decimal Amount { get; set; } 
	public decimal TotalAmount { get; set; }   
	public decimal TotalQuantityInKg { get; set; }

	public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
	public PaymentDto PaymentInfo { get; set; } = new();

	private void MapStatus(string value)
	{
		var matchedStatus = OrderStatus.GetAllStatuses()
			.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

		StatusUI = matchedStatus != null
			? new OrderStatusUIDto { Value = matchedStatus.Value, Color = matchedStatus.Color }
			: new OrderStatusUIDto { Value = value, Color = "#cccccc" };
	}
}

public class OrderItemDto
{
	public string ProductName { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
	public decimal PricePerUnit { get; set; }
	public decimal Quantity { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }
}

public class PaymentDto
{
	public bool WillBePaidInFull { get; set; }
	public int? NumberOfInstallments { get; set; }
	public decimal? AmountPerInstallment { get; set; }
	public DateTime? PaidInFullDate { get; set; }

	public List<InstallmentDto> Installments { get; set; } = new();
}

public class InstallmentDto
{
	public DateTime DueDate { get; set; }
	public decimal Amount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaidOnDate { get; set; }
}
