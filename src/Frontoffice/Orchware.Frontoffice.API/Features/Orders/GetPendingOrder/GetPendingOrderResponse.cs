using Orchware.Frontoffice.API.Features.Orders.Shared.Models;

namespace Orchware.Frontoffice.API.Features.Orders.GetPendingOrder;

public class GetPendingOrderResponse
{
	public decimal Amount { get; set; }
	public OrderStatusUIDto Status { get; set; } = new OrderStatusUIDto();

	public IReadOnlyCollection<OrderDetailsDto>? Details { get; set; }
}

public class OrderDetailsDto
{
	public Guid TempId { get; set; }
	public ProductDto Product { get; set; } = new ProductDto();
	public int ProductId { get; set; }
	public float Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }
}