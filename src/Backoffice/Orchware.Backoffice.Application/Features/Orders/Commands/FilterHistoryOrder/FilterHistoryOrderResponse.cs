using Orchware.Backoffice.Application.Features.Orders.Shared;
using Utilities.Pagination;

namespace Orchware.Backoffice.Application.Features.Orders.Commands.FilterHistoryOrder;

public class FilterHistoryOrderResponse : ResponsePagination<List<OrderDto>>
{
	public int TotalFailed { get; set; }
	public int TotalApproved { get; set; }
}
