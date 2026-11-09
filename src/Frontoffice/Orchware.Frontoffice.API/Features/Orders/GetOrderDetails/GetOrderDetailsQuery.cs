using MediatR;

namespace Orchware.Frontoffice.API.Features.Orders.GetOrderDetails;

public record GetOrderDetailsQuery(string orderId) : IRequest<GetOrderDetailsResponse>;
