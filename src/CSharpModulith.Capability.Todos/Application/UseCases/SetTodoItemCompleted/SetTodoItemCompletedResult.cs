using App.Capability.Todos;

namespace App.Capability.Todos.Application.UseCases.SetTodoItemCompleted;

public readonly record struct SetTodoItemCompletedResult(
    bool IsSuccess,
    TodosFailure? Failure,
    string? Message);
