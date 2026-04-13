using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Model.TodoList.Events;
using App.Capability.Todos.Infrastructure.Persistence.EfCore;
using App.Capability.Todos.Tests.TestInfrastructure;
using App.Shared.Domain;
using App.Shared.Infrastructure.Persistence;

namespace App.Capability.Todos.Tests;

[Trait("Capability", "Todos")]
public sealed class TodoListWriteRepositoryTests
{
    [Fact]
    public async Task persistAsync_persists_new_list_and_restoreAsync_returns_it_with_items()
    {
        // Arrange
        var (connection, context) = TestDbContextFactory.CreateSqliteInMemory();
        await using (connection)
        await using (context)
        {
            var mapper = new TodoListPersistenceMapper();
            var queue = new PostSaveAggregateEventsQueue();
            var dispatch = new CollectingEventDispatch();
            var coordinator = new SaveChangesOnlyDomainEventPersistenceCoordinator();
            var repository = new TodoListWriteRepository(
                context,
                mapper,
                queue,
                coordinator);
            var list = TodoList.Create(TodoListId.From(Guid.NewGuid()), "Groceries");
            list.AddItem(TodoItemId.From(Guid.NewGuid()), "Milk");

            // Act
            await repository.PersistAsync(list);
            await PostSaveDomainEventsTestSupport.DispatchRegisteredEventsAsync(
                queue,
                dispatch);
            var loaded = await repository.RestoreAsync(list.Id);

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal(list.Id.Value, loaded.Id.Value);
            Assert.Equal("Groceries", loaded.Title);
            Assert.Single(loaded.Items);
            Assert.Equal("Milk", loaded.Items[0].Title);
            Assert.False(loaded.Items[0].IsCompleted);
            Assert.Equal(2, dispatch.Dispatched.Count);
            Assert.Contains(dispatch.Dispatched, e => e is TodoListWasCreated);
            Assert.Contains(dispatch.Dispatched, e => e is TodoItemWasAdded);
        }
    }

    [Fact]
    public async Task persistAsync_updates_existing_list_and_items()
    {
        // Arrange
        var (connection, context) = TestDbContextFactory.CreateSqliteInMemory();
        await using (connection)
        await using (context)
        {
            var mapper = new TodoListPersistenceMapper();
            var queue = new PostSaveAggregateEventsQueue();
            var dispatch = new CollectingEventDispatch();
            var coordinator = new SaveChangesOnlyDomainEventPersistenceCoordinator();
            var repository = new TodoListWriteRepository(
                context,
                mapper,
                queue,
                coordinator);
            var list = TodoList.Create(TodoListId.From(Guid.NewGuid()), "Work");
            var itemId = TodoItemId.From(Guid.NewGuid());
            list.AddItem(itemId, "Email");
            await repository.PersistAsync(list);
            await PostSaveDomainEventsTestSupport.DispatchRegisteredEventsAsync(
                queue,
                dispatch);

            list.CompleteItem(itemId);

            // Act
            await repository.PersistAsync(list);
            await PostSaveDomainEventsTestSupport.DispatchRegisteredEventsAsync(
                queue,
                dispatch);
            var loaded = await repository.RestoreAsync(list.Id);

            // Assert
            Assert.NotNull(loaded);
            Assert.True(loaded.Items[0].IsCompleted);
        }
    }
}
