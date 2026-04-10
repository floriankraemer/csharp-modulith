namespace App.Capability.Todos.Domain.Model.TodoList;

public sealed class TodoItem
{
    internal TodoItem(TodoItemId id, string title, bool isCompleted, int sortOrder)
    {
        Id = id;
        Title = title;
        IsCompleted = isCompleted;
        SortOrder = sortOrder;
    }

    public TodoItemId Id { get; }

    public string Title { get; private set; }

    public bool IsCompleted { get; private set; }

    public int SortOrder { get; private set; }

    internal void Rename(string title) => Title = title;

    internal void SetCompleted(bool completed) => IsCompleted = completed;
}
