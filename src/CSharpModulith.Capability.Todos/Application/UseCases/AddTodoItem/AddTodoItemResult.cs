using App.Capability.Todos;

namespace App.Capability.Todos.Application.UseCases.AddTodoItem;

public readonly record struct AddTodoItemResult(
    bool IsSuccess,
    string? ItemId,
    TodosFailure? Failure,
    string? Message);
