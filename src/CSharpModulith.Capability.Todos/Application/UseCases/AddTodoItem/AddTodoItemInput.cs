namespace App.Capability.Todos.Application.UseCases.AddTodoItem;

public readonly record struct AddTodoItemInput(string ListId, string Title);
