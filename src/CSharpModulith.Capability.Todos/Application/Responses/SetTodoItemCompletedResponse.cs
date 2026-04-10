using App.Capability.Todos;

namespace App.Capability.Todos.Application.Responses;

public sealed class SetTodoItemCompletedResponse
{
    private SetTodoItemCompletedResponse(bool isSuccessful, TodosFailure? failure, string? message)
    {
        IsSuccessful = isSuccessful;
        Failure = failure;
        Message = message;
    }

    public bool IsSuccessful { get; }

    public TodosFailure? Failure { get; }

    public string? Message { get; }

    public static SetTodoItemCompletedResponse WithSuccess() =>
        new(true, null, null);

    public static SetTodoItemCompletedResponse WithFailure(TodosFailure failure, string? message = null) =>
        new(false, failure, message);
}
