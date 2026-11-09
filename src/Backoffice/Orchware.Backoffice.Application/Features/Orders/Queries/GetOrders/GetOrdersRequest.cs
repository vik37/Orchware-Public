namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersRequest
{
	public string Status { get; set; } = "All";

	public int? pageIndex { get; set; }
	public int? pageSize { get; set; }
}
