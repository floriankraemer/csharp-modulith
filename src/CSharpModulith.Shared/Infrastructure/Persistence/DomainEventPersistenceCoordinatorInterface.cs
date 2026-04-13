using Microsoft.EntityFrameworkCore;

namespace App.Shared.Infrastructure.Persistence;

/// <summary>
/// Persists tracked EF changes together with pending domain events (e.g. transactional outbox) or delegates to SaveChanges only.
/// </summary>
public interface DomainEventPersistenceCoordinatorInterface
{
    Task SaveChangesWithRegisteredDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken = default);
}
