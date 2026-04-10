using App.Capability.Todos;

namespace App.Capability.Todos.Application.Responses;

public sealed class AddTodoItemResponse
{
    private AddTodoItemResponse(
        bool isSuccessful,
        string? itemId,
        TodosFailure? failure,
        string? message)
    {
        IsSuccessful = isSuccessful;
        ItemId = itemId;
        Failure = failure;
        Message = message;
    }

    public bool IsSuccessful { get; }

    public string? ItemId { get; }

    public TodosFailure? Failure { get; }

    public string? Message { get; }

    public static AddTodoItemResponse WithSuccess(string itemId) =>
        new(true, itemId, null, null);

    public static AddTodoItemResponse WithFailure(TodosFailure failure, string? message = null) =>
        new(false, null, failure, message);
}
