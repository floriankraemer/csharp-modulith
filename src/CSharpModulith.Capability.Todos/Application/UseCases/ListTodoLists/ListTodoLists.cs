using System.Text.Json;
using App.Capability.Todos.Application.Repositories;

namespace App.Capability.Todos.Application.UseCases.ListTodoLists;

/// <summary>
/// Specification:
///
/// - Loads all todo list ids and titles ordered by title then id.
/// - Returns a camelCase JSON array of { listId, title } for API wrapping.
/// </summary>
public sealed class ListTodoLists(TodoListReadRepositoryInterface readRepository)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<ListTodoListsResult> ExecuteAsync(
        ListTodoListsInput input,
        CancellationToken cancellationToken = default)
    {
        var summaries = await readRepository.ListAllAsync(cancellationToken);
        var dto = summaries.Select(s => new SummaryDto(s.ListId, s.Title)).ToArray();
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        return new ListTodoListsResult(
            IsSuccess: true,
            TodoListsJson: json,
            Failure: null,
            Message: null);
    }

    private readonly record struct SummaryDto(string ListId, string Title);
}
