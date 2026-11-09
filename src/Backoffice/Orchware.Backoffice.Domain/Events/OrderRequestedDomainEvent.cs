using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Events;

public record OrderRequestedDomainEvent(Order Order) : IAggregateEvent
{
	public string AggregateName => nameof(Order);
}
