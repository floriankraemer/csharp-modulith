using App.Capability.Todos.Domain.Model.TodoList.Events;
using App.Shared.Application;

namespace App.Capability.Todos.Application.DomainEventHandlers;

public sealed class TodoItemWasAddedHandler : EventHandlerInterface<TodoItemWasAdded>
{
    public Task HandleAsync(
        TodoItemWasAdded domainEvent,
        CancellationToken cancellationToken = default)
    {
        _ = domainEvent;
        _ = cancellationToken;
        return Task.CompletedTask;
    }
}
