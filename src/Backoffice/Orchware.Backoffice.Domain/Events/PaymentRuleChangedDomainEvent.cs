using Orchware.Backoffice.Domain.Entities.Settings;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Events;

public record PaymentRuleChangedDomainEvent(PaymentRules Rules) : IAggregateEvent
{
	public string AggregateName => nameof(PaymentRules);
}
