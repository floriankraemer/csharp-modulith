namespace App.Capability.Todos.Application.ReadModels;

/// <summary>Projection row for listing todo lists (id + title only).</summary>
public readonly record struct TodoListSummary(string ListId, string Title);
