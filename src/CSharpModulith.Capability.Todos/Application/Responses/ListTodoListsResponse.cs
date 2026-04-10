using App.Capability.Todos;

namespace App.Capability.Todos.Application.Responses;

public sealed class ListTodoListsResponse
{
    private ListTodoListsResponse(
        bool isSuccessful,
        string? todoListsJson,
        TodosFailure? failure,
        string? message)
    {
        IsSuccessful = isSuccessful;
        TodoListsJson = todoListsJson;
        Failure = failure;
        Message = message;
    }

    public bool IsSuccessful { get; }

    /// <summary>JSON array of { "listId", "title" }.</summary>
    public string? TodoListsJson { get; }

    public TodosFailure? Failure { get; }

    public string? Message { get; }

    public static ListTodoListsResponse WithSuccess(string todoListsJson) =>
        new(true, todoListsJson, null, null);

    public static ListTodoListsResponse WithFailure(TodosFailure failure, string? message = null) =>
        new(false, null, failure, message);
}
