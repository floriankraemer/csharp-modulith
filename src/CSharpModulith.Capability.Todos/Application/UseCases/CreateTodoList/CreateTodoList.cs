using App.Capability.Todos;
using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Repositories;

namespace App.Capability.Todos.Application.UseCases.CreateTodoList;

/// <summary>
/// Specification:
///
/// - Validates that the title is non-empty.
/// - Creates a new todo list aggregate with a generated id.
/// - Persists the aggregate.
/// </summary>
public sealed class CreateTodoList(TodoListWriteRepositoryInterface repository)
{
    public async Task<CreateTodoListResult> ExecuteAsync(
        CreateTodoListInput input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            return new CreateTodoListResult(
                IsSuccess: false,
                ListId: null,
                Failure: TodosFailure.ValidationError,
                Message: "Title is required.");
        }

        var id = TodoListId.From(Guid.NewGuid());
        var list = TodoList.Create(id, input.Title);
        await repository.PersistAsync(list, cancellationToken);
        return new CreateTodoListResult(
            IsSuccess: true,
            ListId: id.ToString(),
            Failure: null,
            Message: null);
    }
}
