namespace App.Capability.Todos.Application.Requests;

public readonly record struct SetTodoItemCompletedRequest(string ListId, string ItemId, bool Completed);
