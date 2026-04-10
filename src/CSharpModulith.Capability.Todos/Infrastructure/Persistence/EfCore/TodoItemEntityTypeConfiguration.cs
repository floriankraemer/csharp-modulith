using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoItemEntityTypeConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder.ToTable("todo_items");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(500).IsRequired();
        builder.HasIndex(e => new { e.TodoListId, e.SortOrder });
    }
}
