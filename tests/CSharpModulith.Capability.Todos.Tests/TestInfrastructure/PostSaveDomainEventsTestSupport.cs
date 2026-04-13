using App.Shared.Domain;
using App.Shared.Infrastructure.Persistence;

namespace App.Capability.Todos.Tests.TestInfrastructure;

/// <summary>
/// Mirrors <see cref="App.Shared.Infrastructure.Persistence.EfCore.PostSaveDomainEventsSaveChangesInterceptor"/> post-save dispatch for tests without full DI.
/// </summary>
public static class PostSaveDomainEventsTestSupport
{
    public static async Task DispatchRegisteredEventsAsync(
        PostSaveAggregateEventsQueueInterface queue,
        EventDispatchInterface dispatch,
        CancellationToken cancellationToken = default)
    {
        foreach (var aggregate in queue.TakeRegisteredAggregates())
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
    }
}
