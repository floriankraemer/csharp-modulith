using App.Shared.Domain;
using Wolverine;

namespace App.Capability.Todos.Infrastructure.Message;

/// <summary>
/// Adapter: owned dispatch port implemented with Wolverine (Infrastructure only).
/// </summary>
public sealed class WolverineEventDispatch(IMessageBus messageBus) : EventDispatchInterface
{
    public async Task DispatchAsync(
        IReadOnlyList<EventInterface> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await messageBus.PublishAsync(@event);
        }
    }
}
