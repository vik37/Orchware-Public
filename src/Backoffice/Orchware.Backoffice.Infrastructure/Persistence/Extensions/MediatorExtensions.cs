using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Events.Contract;

namespace Orchware.Backoffice.Infrastructure.Persistence.Extensions;

public static class MediatorExtensions
{
	public static async Task DispatchDomainEventsAsync<TContext>(
		this IMediator mediator,
		TContext context,
		CancellationToken cancellationToken = default)
		where TContext : DbContext
	{
		var domainEntities = context.ChangeTracker
			.Entries<IHasDomainEvents>()
			.ToList();

		if (!domainEntities.Any())
			return;

		var domainEvents = domainEntities
			.SelectMany(e => e.Entity.DomainEvents)
			.ToList();

		domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

		var groupedByAggregate = domainEvents
			.OfType<IAggregateEvent>()
			.GroupBy(e => e.AggregateName)
			.ToList();

		var dispatchTasks = groupedByAggregate
			.Select(async group =>
			{
				foreach (var domainEvent in group)
				{
					await mediator.Publish(domainEvent, cancellationToken);
				}
			});

		await Task.WhenAll(dispatchTasks);

		var ungroupedEvents = domainEvents.Except(groupedByAggregate.SelectMany(g => g)).ToList();
		foreach (var domainEvent in ungroupedEvents)
		{
			await mediator.Publish(domainEvent, cancellationToken);
		}
	}
}