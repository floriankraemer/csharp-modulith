namespace App.Shared.Domain;

/// <summary>
/// Event-only aggregate base. Identity and persistence mapping stay in concrete aggregates.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<EventInterface> _pendingEvents = new();

    public IReadOnlyCollection<EventInterface> PendingEvents => _pendingEvents.AsReadOnly();

    protected void Raise(EventInterface @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        _pendingEvents.Add(@event);
    }

    public void ClearPendingEvents() => _pendingEvents.Clear();
}
