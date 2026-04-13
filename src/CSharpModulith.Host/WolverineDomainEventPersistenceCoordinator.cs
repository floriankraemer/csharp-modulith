using System.Collections.Generic;
using App.Shared.Domain;
using App.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace CSharpModulith.Host;

/// <summary>
/// PostgreSQL + Wolverine: enrolls the active <see cref="AppDbContext"/>, publishes pending domain events to the EF outbox,
/// then commits entity changes and outbox rows in one unit of work.
/// </summary>
public sealed class WolverineDomainEventPersistenceCoordinator(
    IDbContextOutbox<AppDbContext> outbox,
    PostSaveAggregateEventsQueueInterface queue) : DomainEventPersistenceCoordinatorInterface
{
    public async Task SaveChangesWithRegisteredDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        var appContext = (AppDbContext)context;
        if (!ReferenceEquals(appContext, outbox.DbContext))
        {
            throw new InvalidOperationException(
                "The active DbContext must match IDbContextOutbox<AppDbContext>.DbContext for transactional outbox.");
        }

        var aggregates = queue.TakeRegisteredAggregates();
        var snapshots = new List<(AggregateRoot Aggregate, List<EventInterface> Events)>();
        foreach (var aggregate in aggregates)
        {
            snapshots.Add((aggregate, aggregate.PendingEvents.ToList()));
        }

        foreach (var (_, events) in snapshots)
        {
            foreach (var @event in events)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await outbox.PublishAsync(@event);
            }
        }

        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);

        foreach (var (aggregate, _) in snapshots)
        {
            aggregate.ClearPendingEvents();
        }
    }
}
