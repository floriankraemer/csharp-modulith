namespace App.Capability.Todos.Application.Requests;

public readonly record struct AddTodoItemRequest(string ListId, string Title);
