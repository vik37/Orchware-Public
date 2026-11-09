using MediatR;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<GetOrdersResponse>;
