using App.Capability.Todos;

namespace App.Capability.Todos.Application.Responses;

public sealed class CreateTodoListResponse
{
    private CreateTodoListResponse(
        bool isSuccessful,
        string? listId,
        TodosFailure? failure,
        string? message)
    {
        IsSuccessful = isSuccessful;
        ListId = listId;
        Failure = failure;
        Message = message;
    }

    public bool IsSuccessful { get; }

    public string? ListId { get; }

    public TodosFailure? Failure { get; }

    public string? Message { get; }

    public static CreateTodoListResponse WithSuccess(string listId) =>
        new(true, listId, null, null);

    public static CreateTodoListResponse WithFailure(TodosFailure failure, string? message = null) =>
        new(false, null, failure, message);
}
