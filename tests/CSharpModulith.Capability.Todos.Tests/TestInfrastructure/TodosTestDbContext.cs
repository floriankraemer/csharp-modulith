using App.Capability.Todos.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Capability.Todos.Tests.TestInfrastructure;

public sealed class TodosTestDbContext(DbContextOptions<TodosTestDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodosInfrastructureMarker).Assembly);
    }
}
