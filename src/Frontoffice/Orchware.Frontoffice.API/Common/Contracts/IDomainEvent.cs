using MediatR;

namespace Orchware.Frontoffice.API.Common.Contracts;

public interface IDomainEvent : INotification
{
	public DateTime OrderedDate { get; }
}
