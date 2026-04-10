using App.Capability.Todos;
using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Repositories;

namespace App.Capability.Todos.Application.UseCases.AddTodoItem;

/// <summary>
/// Specification:
///
/// - Resolves the list id and loads the aggregate.
/// - Validates item title.
/// - Adds a new item with a generated id and persists.
/// </summary>
public sealed class AddTodoItem(TodoListWriteRepositoryInterface repository)
{
    public async Task<AddTodoItemResult> ExecuteAsync(
        AddTodoItemInput input,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(input.ListId, out var listGuid))
        {
            return new AddTodoItemResult(
                IsSuccess: false,
                ItemId: null,
                Failure: TodosFailure.ValidationError,
                Message: "List id is not a valid GUID.");
        }

        if (string.IsNullOrWhiteSpace(input.Title))
        {
            return new AddTodoItemResult(
                IsSuccess: false,
                ItemId: null,
                Failure: TodosFailure.ValidationError,
                Message: "Title is required.");
        }

        var listId = TodoListId.From(listGuid);
        var list = await repository.RestoreAsync(listId, cancellationToken);
        if (list is null)
        {
            return new AddTodoItemResult(
                IsSuccess: false,
                ItemId: null,
                Failure: TodosFailure.ListNotFound,
                Message: "Todo list was not found.");
        }

        var itemId = TodoItemId.From(Guid.NewGuid());
        list.AddItem(itemId, input.Title);
        await repository.PersistAsync(list, cancellationToken);
        return new AddTodoItemResult(
            IsSuccess: true,
            ItemId: itemId.ToString(),
            Failure: null,
            Message: null);
    }
}
