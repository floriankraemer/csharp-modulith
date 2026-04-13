using App.Shared.Domain;

namespace App.Capability.Todos.Tests.TestInfrastructure;

public sealed class CollectingEventDispatch : EventDispatchInterface
{
    private readonly List<EventInterface> _dispatched = new();

    public IReadOnlyList<EventInterface> Dispatched => _dispatched;

    public Task DispatchAsync(
        IReadOnlyList<EventInterface> events,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        _dispatched.AddRange(events);
        return Task.CompletedTask;
    }
}
