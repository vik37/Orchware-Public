using MediatR;

namespace Orchware.Backoffice.Domain.Events.Contract;

public interface IHasDomainEvents
{
	IReadOnlyCollection<INotification> DomainEvents { get; }
	void AddDomainEvent(INotification eventItem);
	void ClearDomainEvents();
}
