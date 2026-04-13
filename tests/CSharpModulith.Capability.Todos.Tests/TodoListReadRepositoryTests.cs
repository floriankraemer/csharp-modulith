using App.Capability.Todos.Application.Repositories;
using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Infrastructure.Persistence.EfCore;
using App.Capability.Todos.Tests.TestInfrastructure;

namespace App.Capability.Todos.Tests;

[Trait("Capability", "Todos")]
public sealed class TodoListReadRepositoryTests
{
    [Fact]
    public async Task listAllAsync_returns_empty_when_no_lists()
    {
        // Arrange
        var (connection, context) = TestDbContextFactory.CreateSqliteInMemory();
        await using (connection)
        await using (context)
        {
            var repository = new TodoListReadRepository(context);

            // Act
            var result = await repository.ListAllAsync();

            // Assert
            Assert.Empty(result);
        }
    }

    [Fact]
    public async Task listAllAsync_returns_summaries_ordered_by_title_then_id()
    {
        // Arrange
        var (connection, context) = TestDbContextFactory.CreateSqliteInMemory();
        await using (connection)
        await using (context)
        {
            var mapper = new TodoListPersistenceMapper();
            var write = new TodoListWriteRepository(context, mapper);
            var idB = TodoListId.From(Guid.NewGuid());
            var idA = TodoListId.From(Guid.NewGuid());
            await write.PersistAsync(TodoList.Create(idB, "Beta"));
            await write.PersistAsync(TodoList.Create(idA, "Alpha"));
            var repository = new TodoListReadRepository(context);

            // Act
            var result = await repository.ListAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Alpha", result[0].Title);
            Assert.Equal("Beta", result[1].Title);
            Assert.Equal(idA.Value.ToString(), result[0].ListId);
            Assert.Equal(idB.Value.ToString(), result[1].ListId);
        }
    }
}
