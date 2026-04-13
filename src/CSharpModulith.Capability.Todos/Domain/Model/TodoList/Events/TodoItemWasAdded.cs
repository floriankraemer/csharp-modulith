using App.Shared.Domain;

namespace App.Capability.Todos.Domain.Model.TodoList.Events;

public sealed record TodoItemWasAdded(
    Guid EventId,
    DateTimeOffset OccurredOn,
    TodoListId ListId,
    TodoItemId ItemId,
    string Title) : EventInterface;
