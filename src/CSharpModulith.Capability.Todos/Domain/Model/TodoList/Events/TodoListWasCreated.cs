using App.Shared.Domain;

namespace App.Capability.Todos.Domain.Model.TodoList.Events;

public sealed record TodoListWasCreated(
    Guid EventId,
    DateTimeOffset OccurredOn,
    TodoListId ListId,
    string Title) : EventInterface;
