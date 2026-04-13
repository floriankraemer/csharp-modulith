namespace App.Shared.Domain;

/// <summary>
/// Scoped queue of aggregate roots that should have their pending domain events dispatched after SaveChanges succeeds.
/// </summary>
public interface PostSaveAggregateEventsQueueInterface
{
    void Register(AggregateRoot aggregate);

    /// <summary>
    /// Returns and clears the current registration list. Call after a successful save.
    /// </summary>
    IReadOnlyList<AggregateRoot> TakeRegisteredAggregates();
}
