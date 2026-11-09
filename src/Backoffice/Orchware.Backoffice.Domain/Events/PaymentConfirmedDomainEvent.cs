using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Events;

public record PaymentConfirmedDomainEvent(List<OrderDetails> Details) : IAggregateEvent
{
	public string AggregateName => nameof(Order);
}
