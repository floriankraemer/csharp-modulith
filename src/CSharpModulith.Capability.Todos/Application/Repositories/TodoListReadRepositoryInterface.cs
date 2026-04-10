using App.Capability.Todos.Application.ReadModels;

namespace App.Capability.Todos.Application.Repositories;

public interface TodoListReadRepositoryInterface
{
    Task<IReadOnlyList<TodoListSummary>> ListAllAsync(CancellationToken cancellationToken = default);
}
