namespace Orchware.Backoffice.Application.Features.Orders.Shared;

public class OrderDto
{
	public Guid OrderId { get; set; }
	public string OrderNumber { get; set; } = string.Empty;
	public DateTime OrderedDate { get; set; }
	public string Buyer { get; set; } = string.Empty;
	public string Company { get; set; } = string.Empty;
	public bool WillBePaidInFull { get; set; }
	public DateTime Deadline { get; set; }
	public int? Discount { get; set; }
	public string Status { get; set; } = string.Empty;
	public OrderStatusUIDto OrderStatus { get; set; } = new();
	public decimal Amount { get; set; }
	public decimal TotalAmount { get; set; }
	public decimal TotalQuantityInKg { get; set; }
}
