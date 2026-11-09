using MediatR;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;
using Utilities.Pagination;

namespace Orchware.Frontoffice.API.Features.Orders.FilterHistoryOrder;

public record FilterHistoryOrderCommand(FilterHistoryOrderRequest Filter) : IRequest<ResponsePagination<List<OrderDto>>>;
