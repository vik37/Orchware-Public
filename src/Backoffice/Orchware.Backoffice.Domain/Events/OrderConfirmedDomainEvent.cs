using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Events;

public record OrderConfirmedDomainEvent(Order Order) : IAggregateEvent
{
	public string AggregateName => nameof(Order);
}
