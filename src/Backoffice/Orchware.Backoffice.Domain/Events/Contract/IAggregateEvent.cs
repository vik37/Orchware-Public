using MediatR;

namespace Orchware.Backoffice.Domain.Events.Contract;

public interface IAggregateEvent : INotification
{
	string AggregateName { get; }
}
