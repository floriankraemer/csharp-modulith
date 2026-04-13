using App.Capability.Todos.Domain.Model.TodoList.Events;
using App.Shared.Application;

namespace App.Capability.Todos.Application.DomainEventHandlers;

/// <summary>
/// Minimal in-process reaction to TodoListWasCreated (extend with real side effects later).
/// </summary>
public sealed class TodoListWasCreatedHandler : EventHandlerInterface<TodoListWasCreated>
{
    public Task HandleAsync(
        TodoListWasCreated domainEvent,
        CancellationToken cancellationToken = default)
    {
        _ = domainEvent;
        _ = cancellationToken;
        return Task.CompletedTask;
    }
}
