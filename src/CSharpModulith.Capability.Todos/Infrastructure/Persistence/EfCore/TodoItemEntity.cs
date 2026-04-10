namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoItemEntity
{
    public Guid Id { get; set; }

    public Guid TodoListId { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int SortOrder { get; set; }

    public TodoListEntity? List { get; set; }
}
