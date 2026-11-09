using MediatR;

namespace Orchware.Frontoffice.API.Features.Orders.CancelOrder;

public record CancelOrderCommand : IRequest<Unit>
{
	public Guid OrderId { get; init; }
	public string Reason { get; init; } = string.Empty;
}
