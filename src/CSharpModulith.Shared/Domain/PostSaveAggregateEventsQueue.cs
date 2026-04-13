namespace App.Shared.Domain;

/// <summary>
/// Scoped buffer of aggregate roots whose domain events should be dispatched after SaveChanges succeeds.
/// </summary>
public sealed class PostSaveAggregateEventsQueue : PostSaveAggregateEventsQueueInterface
{
    private readonly List<AggregateRoot> _aggregates = new();

    public void Register(AggregateRoot aggregate)
    {
        ArgumentNullException.ThrowIfNull(aggregate);
        _aggregates.Add(aggregate);
    }

    public IReadOnlyList<AggregateRoot> TakeRegisteredAggregates()
    {
        if (_aggregates.Count == 0)
        {
            return Array.Empty<AggregateRoot>();
        }

        var copy = _aggregates.ToArray();
        _aggregates.Clear();
        return copy;
    }
}
