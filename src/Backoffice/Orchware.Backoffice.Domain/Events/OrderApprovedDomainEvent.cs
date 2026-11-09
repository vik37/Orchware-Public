using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Events;

public record OrderApprovedDomainEvent(string OrderNumber) : IAggregateEvent
{
	public string AggregateName => nameof(Order);
}
