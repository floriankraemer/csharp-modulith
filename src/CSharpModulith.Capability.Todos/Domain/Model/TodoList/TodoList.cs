using App.Capability.Todos.Domain;

namespace App.Capability.Todos.Domain.Model.TodoList;

public sealed class TodoList
{
    private readonly List<TodoItem> _items = new();

    private TodoList(TodoListId id, string title)
    {
        Id = id;
        Title = title;
    }

    public TodoListId Id { get; }

    public string Title { get; private set; }

    public IReadOnlyList<TodoItem> Items => _items;

    public static TodoList Create(TodoListId id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw InvariantViolationException.Because("Todo list title must not be empty.");
        }

        return new TodoList(id, title.Trim());
    }

    internal static TodoList Restore(TodoListId id, string title, IReadOnlyList<TodoItem> items)
    {
        var list = new TodoList(id, title);
        foreach (var item in items)
        {
            list._items.Add(item);
        }

        return list;
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw InvariantViolationException.Because("Todo list title must not be empty.");
        }

        Title = title.Trim();
    }

    public TodoItem AddItem(TodoItemId itemId, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw InvariantViolationException.Because("Todo item title must not be empty.");
        }

        var nextOrder = _items.Count == 0 ? 0 : _items.Max(i => i.SortOrder) + 1;
        var item = new TodoItem(itemId, title.Trim(), isCompleted: false, sortOrder: nextOrder);
        _items.Add(item);
        return item;
    }

    public void CompleteItem(TodoItemId itemId)
    {
        var item = FindItem(itemId);
        item.SetCompleted(true);
    }

    public void UncompleteItem(TodoItemId itemId)
    {
        var item = FindItem(itemId);
        item.SetCompleted(false);
    }

    public void RemoveItem(TodoItemId itemId)
    {
        var item = FindItem(itemId);
        _items.Remove(item);
    }

    public TodoItem? FindItemById(TodoItemId itemId) =>
        _items.FirstOrDefault(i => i.Id.Value == itemId.Value);

    private TodoItem FindItem(TodoItemId itemId)
    {
        var item = FindItemById(itemId);
        if (item is null)
        {
            throw InvariantViolationException.Because("Todo item is not part of this list.");
        }

        return item;
    }

}
