using App.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.Shared.Tests.Domain;

public sealed class SaveChangesOnlyDomainEventPersistenceCoordinatorTests
{
    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }
    }

    [Fact]
    public async Task saveChangesWithRegisteredDomainEventsAsync_persists_tracked_changes()
    {
        // Arrange
        await using var context = new TestDbContext(
            new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);
        context.TestEntities.Add(new TestEntity { Id = 1 });
        var coordinator = new SaveChangesOnlyDomainEventPersistenceCoordinator();

        // Act
        await coordinator.SaveChangesWithRegisteredDomainEventsAsync(context);

        // Assert
        Assert.Equal(1, await context.TestEntities.AsNoTracking().CountAsync());
    }
}
