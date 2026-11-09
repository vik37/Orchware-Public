using MediatR;
using Orchware.Backoffice.Application.Features.Orders.Shared;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.OrderStatuses;

public record OrderStatusesQuery : IRequest<List<OrderStatusUIDto>>;

public class OrderStatusesQueryHandler : IRequestHandler<OrderStatusesQuery, List<OrderStatusUIDto>>
{
	public Task<List<OrderStatusUIDto>> Handle(OrderStatusesQuery request, CancellationToken cancellationToken)
	{
		var statuses = MapStatuses(OrderStatus.GetAllStatuses());

		return Task.FromResult(statuses.ToList());
	}

	private IEnumerable<OrderStatusUIDto> MapStatuses(IEnumerable<OrderStatus> statuses)
	{
		foreach (var status in statuses)
		{
			yield return new OrderStatusUIDto
			{
				Color = status.Color,
				Value = status.Value
			};
		}
	}
}
