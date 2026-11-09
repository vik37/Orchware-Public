namespace Orchware.Frontoffice.API.Features.Orders.Shared.Models;

public class OrderDto
{
	public Guid OrderId { get; set; }
	public string OrderNumber { get; set; } = string.Empty;
	public DateTime OrderDate { get; set; }
	public int? Discount { get; set; }
	public string Status { get; set; } = string.Empty;
	public OrderStatusUIDto OrderStatus { get; set; } = new();
	public decimal Amount { get; set; }
	public decimal TotalAmount { get; set; }
	public decimal TotalQuantityInKg { get; set; }
}
