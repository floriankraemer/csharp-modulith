using App.Capability.Todos;

namespace App.Capability.Todos.Application.UseCases.ListTodoLists;

public readonly record struct ListTodoListsResult(
    bool IsSuccess,
    string? TodoListsJson,
    TodosFailure? Failure,
    string? Message);
