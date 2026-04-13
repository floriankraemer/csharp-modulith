using App.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

/// <summary>
/// After a successful SaveChanges, dispatches pending domain events for aggregates registered on the scoped queue.
/// Resolves dependencies from the active DbContext service provider (scoped lifetime).
/// </summary>
public sealed class PostSaveDomainEventsSaveChangesInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var saved = await base.SavedChangesAsync(
            eventData,
            result,
            cancellationToken);

        if (result <= 0 || eventData.Context is null)
        {
            return saved;
        }

        var queue = eventData.Context.GetService<PostSaveAggregateEventsQueueInterface>();
        var dispatch = eventData.Context.GetService<EventDispatchInterface>();
        if (queue is null || dispatch is null)
        {
            return saved;
        }

        var aggregates = queue.TakeRegisteredAggregates();
        foreach (var aggregate in aggregates)
        {
            var events = aggregate.PendingEvents.ToList();
            aggregate.ClearPendingEvents();
            if (events.Count > 0)
            {
                await dispatch.DispatchAsync(
                    events,
                    cancellationToken);
            }
        }

        return saved;
    }
}
