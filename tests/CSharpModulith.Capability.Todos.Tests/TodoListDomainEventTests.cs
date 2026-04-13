using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Model.TodoList.Events;

namespace App.Capability.Todos.Tests;

[Trait("Capability", "Todos")]
public sealed class TodoListDomainEventTests
{
    [Fact]
    public void create_emits_TodoListWasCreated_with_expected_payload()
    {
        // Arrange
        var id = TodoListId.From(Guid.NewGuid());

        // Act
        var list = TodoList.Create(id, "  Inbox  ");

        // Assert
        var pending = list.PendingEvents.ToList();
        Assert.Single(pending);
        var created = Assert.IsType<TodoListWasCreated>(pending[0]);
        Assert.Equal(id, created.ListId);
        Assert.Equal("Inbox", created.Title);
        Assert.NotEqual(Guid.Empty, created.EventId);
    }

    [Fact]
    public void addItem_emits_TodoItemWasAdded()
    {
        // Arrange
        var list = TodoList.Create(TodoListId.From(Guid.NewGuid()), "List");
        list.ClearPendingEvents();
        var itemId = TodoItemId.From(Guid.NewGuid());

        // Act
        list.AddItem(itemId, "Task");

        // Assert
        var added = Assert.Single(list.PendingEvents.OfType<TodoItemWasAdded>());
        Assert.Equal(itemId, added.ItemId);
        Assert.Equal("Task", added.Title);
    }

    [Fact]
    public void restore_does_not_leave_pending_events()
    {
        // Arrange
        var id = TodoListId.From(Guid.NewGuid());
        var item = new TodoItem(
            TodoItemId.From(Guid.NewGuid()),
            "X",
            isCompleted: false,
            sortOrder: 0);

        // Act
        var list = TodoList.Restore(
            id,
            "Title",
            new[] { item });

        // Assert
        Assert.Empty(list.PendingEvents);
    }
}
