using App.Capability.Todos.Domain.Model.TodoList;
using App.Capability.Todos.Domain.Repositories;
using App.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoListWriteRepository(
    DbContext context,
    TodoListPersistenceMapper mapper,
    PostSaveAggregateEventsQueueInterface postSaveAggregateEventsQueue) : TodoListWriteRepositoryInterface
{
    public async Task<TodoList?> RestoreAsync(TodoListId listId, CancellationToken cancellationToken = default)
    {
        var listGuid = listId.Value;
        var entity = await context.Set<TodoListEntity>()
            .Include(l => l.Items)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Id == listGuid, cancellationToken);

        return entity is null ? null : mapper.ToDomain(entity);
    }

    public async Task PersistAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        var set = context.Set<TodoListEntity>();
        var aggregateListId = list.Id.Value;
        var existing = await set
            .Include(l => l.Items)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Id == aggregateListId, cancellationToken);

        if (existing is null)
        {
            set.Add(mapper.ToNewEntity(list));
        }
        else
        {
            mapper.ApplyToTrackedEntity(list, existing);
        }

        postSaveAggregateEventsQueue.Register(list);
        await context.SaveChangesAsync(cancellationToken);
    }
}
