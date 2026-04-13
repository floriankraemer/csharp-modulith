using App.Capability.Todos.Domain.Model.TodoList.Events;
using App.Shared.Application;

namespace App.Capability.Todos.Infrastructure.Message;

/// <summary>
/// Wolverine-discoverable entry points that forward to application-owned <see cref="EventHandlerInterface{TEvent}"/> implementations.
/// </summary>
public static class TodosDomainEventWolverineBridge
{
    public static Task Handle(
        TodoListWasCreated message,
        EventHandlerInterface<TodoListWasCreated> handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(
            message,
            cancellationToken);

    public static Task Handle(
        TodoItemWasAdded message,
        EventHandlerInterface<TodoItemWasAdded> handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(
            message,
            cancellationToken);
}
