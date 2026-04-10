using App.Capability.Todos;

namespace App.Capability.Todos.Application.Responses;

public sealed class GetTodoListResponse
{
    private GetTodoListResponse(
        bool isSuccessful,
        string? listId,
        string? title,
        string? itemsJson,
        TodosFailure? failure,
        string? message)
    {
        IsSuccessful = isSuccessful;
        ListId = listId;
        Title = title;
        ItemsJson = itemsJson;
        Failure = failure;
        Message = message;
    }

    public bool IsSuccessful { get; }

    public string? ListId { get; }

    public string? Title { get; }

    /// <summary>JSON array of { "id", "title", "completed", "sortOrder" }.</summary>
    public string? ItemsJson { get; }

    public TodosFailure? Failure { get; }

    public string? Message { get; }

    public static GetTodoListResponse WithSuccess(string listId, string title, string itemsJson) =>
        new(true, listId, title, itemsJson, null, null);

    public static GetTodoListResponse WithFailure(TodosFailure failure, string? message = null) =>
        new(false, null, null, null, failure, message);
}
