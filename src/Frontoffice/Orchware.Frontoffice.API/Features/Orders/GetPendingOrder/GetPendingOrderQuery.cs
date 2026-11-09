using MediatR;

namespace Orchware.Frontoffice.API.Features.Orders.GetPendingOrder;

public record GetPendingOrderQuery : IRequest<GetPendingOrderResponse?>;
