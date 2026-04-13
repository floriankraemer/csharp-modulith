using Microsoft.EntityFrameworkCore;

namespace App.Shared.Domain;

/// <summary>
/// Standard EF SaveChanges; domain events are dispatched separately (e.g. via SaveChangesInterceptor + <see cref="EventDispatchInterface"/>).
/// </summary>
public sealed class SaveChangesOnlyDomainEventPersistenceCoordinator : DomainEventPersistenceCoordinatorInterface
{
    public Task SaveChangesWithRegisteredDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.SaveChangesAsync(cancellationToken);
    }
}
