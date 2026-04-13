using App.Shared.Domain;

namespace App.Shared.Application;

/// <summary>
/// Application-layer reaction to a single domain event type (no messaging framework types).
/// </summary>
public interface EventHandlerInterface<in TEvent>
    where TEvent : class, EventInterface
{
    Task HandleAsync(
        TEvent domainEvent,
        CancellationToken cancellationToken = default);
}
