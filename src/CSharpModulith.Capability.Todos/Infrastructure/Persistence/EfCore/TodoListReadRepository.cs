using App.Capability.Todos.Application.ReadModels;
using App.Capability.Todos.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace App.Capability.Todos.Infrastructure.Persistence.EfCore;

public sealed class TodoListReadRepository(DbContext context) : TodoListReadRepositoryInterface
{
    public async Task<IReadOnlyList<TodoListSummary>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await context.Set<TodoListEntity>()
            .AsNoTracking()
            .OrderBy(l => l.Title)
            .ThenBy(l => l.Id)
            .Select(l => new { l.Id, l.Title })
            .ToListAsync(cancellationToken);

        return rows.ConvertAll(r => new TodoListSummary(r.Id.ToString(), r.Title));
    }
}
