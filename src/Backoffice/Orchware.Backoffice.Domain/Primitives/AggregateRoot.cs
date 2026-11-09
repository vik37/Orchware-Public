using MediatR;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Domain.Primitives;

public abstract class AggregateRoot<TID> : Entity<TID>, IHasDomainEvents, IAggregateRoot
{
	public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;
	private readonly List<INotification> _domainEvents = new();

	public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

	public void ClearDomainEvents() => _domainEvents.Clear();
}
