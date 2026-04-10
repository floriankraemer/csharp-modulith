using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoListEntityTypeConfiguration : IEntityTypeConfiguration<TodoListEntity>
{
    public void Configure(EntityTypeBuilder<TodoListEntity> builder)
    {
        builder.ToTable("todo_lists");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(500).IsRequired();
        builder.HasMany(e => e.Items)
            .WithOne(e => e.List!)
            .HasForeignKey(e => e.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
