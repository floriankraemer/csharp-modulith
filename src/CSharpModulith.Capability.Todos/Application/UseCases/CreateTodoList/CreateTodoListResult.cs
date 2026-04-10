using App.Capability.Todos;

namespace App.Capability.Todos.Application.UseCases.CreateTodoList;

public readonly record struct CreateTodoListResult(
    bool IsSuccess,
    string? ListId,
    TodosFailure? Failure,
    string? Message);
