using Orchware.Backoffice.Application.Features.Orders.Shared;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersResponse
{
	public decimal TotalAmount { get; set; }
	public int TotalCount { get; set; }

	public List<OrderDto> Orders { get; set; } = new();
}
