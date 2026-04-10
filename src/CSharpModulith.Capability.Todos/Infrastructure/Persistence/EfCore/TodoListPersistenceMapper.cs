using App.Capability.Todos.Domain.Model.TodoList;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoListPersistenceMapper
{
    public TodoList ToDomain(TodoListEntity entity)
    {
        var items = entity.Items
            .OrderBy(i => i.SortOrder)
            .Select(i => new TodoItem(
                id: TodoItemId.From(i.Id),
                title: i.Title,
                isCompleted: i.IsCompleted,
                sortOrder: i.SortOrder))
            .ToList();

        return TodoList.Restore(
            id: TodoListId.From(entity.Id),
            title: entity.Title,
            items: items);
    }

    public TodoListEntity ToNewEntity(TodoList list)
    {
        var entity = new TodoListEntity
        {
            Id = list.Id.Value,
            Title = list.Title,
            Items = new List<TodoItemEntity>(),
        };

        foreach (var item in list.Items)
        {
            entity.Items.Add(ToItemEntity(item, list.Id.Value));
        }

        return entity;
    }

    public void ApplyToTrackedEntity(TodoList list, TodoListEntity tracked)
    {
        tracked.Title = list.Title;
        var domainIds = list.Items.Select(i => i.Id.Value).ToHashSet();

        foreach (var orphan in tracked.Items.Where(e => !domainIds.Contains(e.Id)).ToList())
        {
            tracked.Items.Remove(orphan);
        }

        foreach (var domainItem in list.Items)
        {
            var existing = tracked.Items.FirstOrDefault(e => e.Id == domainItem.Id.Value);
            if (existing is null)
            {
                tracked.Items.Add(ToItemEntity(domainItem, list.Id.Value));
                continue;
            }

            existing.Title = domainItem.Title;
            existing.IsCompleted = domainItem.IsCompleted;
            existing.SortOrder = domainItem.SortOrder;
        }
    }

    private static TodoItemEntity ToItemEntity(TodoItem item, Guid listId) =>
        new()
        {
            Id = item.Id.Value,
            TodoListId = listId,
            Title = item.Title,
            IsCompleted = item.IsCompleted,
            SortOrder = item.SortOrder,
        };
}
