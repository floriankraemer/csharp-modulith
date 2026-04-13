namespace App.Shared.Domain;

/// <summary>
/// Port for publishing domain events after successful persistence (adapter may use Wolverine, outbox, etc.).
/// </summary>
public interface EventDispatchInterface
{
    Task DispatchAsync(
        IReadOnlyList<EventInterface> events,
        CancellationToken cancellationToken = default);
}
