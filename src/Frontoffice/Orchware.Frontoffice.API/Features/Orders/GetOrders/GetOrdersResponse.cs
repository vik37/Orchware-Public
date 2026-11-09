using Orchware.Frontoffice.API.Features.Orders.Shared.Models;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrders;

public class GetOrdersResponse
{
	public decimal TotalAmount { get; set; }
	public int TotalCount { get; set; }

	public List<OrderDto> Orders { get; set; } = new();
}
