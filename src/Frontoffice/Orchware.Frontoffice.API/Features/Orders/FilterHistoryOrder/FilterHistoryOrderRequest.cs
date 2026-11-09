using Utilities.Pagination;

namespace Orchware.Frontoffice.API.Features.Orders.FilterHistoryOrder;

public class FilterHistoryOrderRequest : RequestPagination
{
	public string OrderStatus { get; set; } = string.Empty;
	public DateTime? From { get; set; }
	public DateTime? To { get; set; }
}
