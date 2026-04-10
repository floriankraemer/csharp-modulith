using App.Capability.Todos;

namespace App.Capability.Todos.Application.UseCases.GetTodoList;

public readonly record struct GetTodoListResult(
    bool IsSuccess,
    string? ListId,
    string? Title,
    string? ItemsJson,
    TodosFailure? Failure,
    string? Message);
