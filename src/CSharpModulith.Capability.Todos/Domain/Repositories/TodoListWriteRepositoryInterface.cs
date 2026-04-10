using App.Capability.Todos.Domain.Model.TodoList;

namespace App.Capability.Todos.Domain.Repositories;

public interface TodoListWriteRepositoryInterface
{
    Task<TodoList?> RestoreAsync(TodoListId id, CancellationToken cancellationToken = default);

    Task PersistAsync(TodoList list, CancellationToken cancellationToken = default);
}
