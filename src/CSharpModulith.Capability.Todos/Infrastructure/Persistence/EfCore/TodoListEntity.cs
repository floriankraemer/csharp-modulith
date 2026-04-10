namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoListEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<TodoItemEntity> Items { get; set; } = new();
}
