using MediatR;

namespace Orchware.Frontoffice.API.Features.Orders.DeletePendingOrder;

public record DeletePendingOrderCommand(string TempId) : IRequest<Unit>;
