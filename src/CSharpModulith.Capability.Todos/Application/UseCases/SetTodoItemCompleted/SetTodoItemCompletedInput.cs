namespace App.Capability.Todos.Application.UseCases.SetTodoItemCompleted;

public readonly record struct SetTodoItemCompletedInput(string ListId, string ItemId, bool Completed);
