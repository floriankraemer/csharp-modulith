using System.Text.Json;
using App.Capability.Todos;
using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Repositories;

namespace App.Capability.Todos.Application.UseCases.GetTodoList;

/// <summary>
/// Specification:
///
/// - Loads the list aggregate with items.
/// - Returns list metadata and a JSON payload of items ordered by sort order.
/// </summary>
public sealed class GetTodoList(TodoListWriteRepositoryInterface repository)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<GetTodoListResult> ExecuteAsync(
        GetTodoListInput input,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(input.ListId, out var listGuid))
        {
            return new GetTodoListResult(
                IsSuccess: false,
                ListId: null,
                Title: null,
                ItemsJson: null,
                Failure: TodosFailure.ValidationError,
                Message: "List id is not a valid GUID.");
        }

        var listId = TodoListId.From(listGuid);
        var list = await repository.RestoreAsync(listId, cancellationToken);
        if (list is null)
        {
            return new GetTodoListResult(
                IsSuccess: false,
                ListId: null,
                Title: null,
                ItemsJson: null,
                Failure: TodosFailure.ListNotFound,
                Message: "Todo list was not found.");
        }

        var dto = list.Items.Select(i => new ItemDto(
            Id: i.Id.ToString(),
            Title: i.Title,
            Completed: i.IsCompleted,
            SortOrder: i.SortOrder)).ToArray();

        var json = JsonSerializer.Serialize(dto, JsonOptions);
        return new GetTodoListResult(
            IsSuccess: true,
            ListId: list.Id.ToString(),
            Title: list.Title,
            ItemsJson: json,
            Failure: null,
            Message: null);
    }

    private readonly record struct ItemDto(string Id, string Title, bool Completed, int SortOrder);
}
