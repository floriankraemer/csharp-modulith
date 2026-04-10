using App.Capability.Todos;
using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Repositories;

namespace App.Capability.Todos.Application.UseCases.SetTodoItemCompleted;

/// <summary>
/// Specification:
///
/// - Loads the list aggregate.
/// - Ensures the item exists on the list.
/// - Sets completion state and persists.
/// </summary>
public sealed class SetTodoItemCompleted(TodoListWriteRepositoryInterface repository)
{
    public async Task<SetTodoItemCompletedResult> ExecuteAsync(
        SetTodoItemCompletedInput input,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(input.ListId, out var listGuid) || !Guid.TryParse(input.ItemId, out var itemGuid))
        {
            return new SetTodoItemCompletedResult(
                IsSuccess: false,
                Failure: TodosFailure.ValidationError,
                Message: "List id and item id must be valid GUIDs.");
        }

        var listId = TodoListId.From(listGuid);
        var list = await repository.RestoreAsync(listId, cancellationToken);
        if (list is null)
        {
            return new SetTodoItemCompletedResult(
                IsSuccess: false,
                Failure: TodosFailure.ListNotFound,
                Message: "Todo list was not found.");
        }

        var itemId = TodoItemId.From(itemGuid);
        if (list.FindItemById(itemId) is null)
        {
            return new SetTodoItemCompletedResult(
                IsSuccess: false,
                Failure: TodosFailure.ItemNotFound,
                Message: "Todo item was not found on this list.");
        }

        if (input.Completed)
        {
            list.CompleteItem(itemId);
        }
        else
        {
            list.UncompleteItem(itemId);
        }

        await repository.PersistAsync(list, cancellationToken);
        return new SetTodoItemCompletedResult(IsSuccess: true, Failure: null, Message: null);
    }
}
