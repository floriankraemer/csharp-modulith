using App.Capability.Todos.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CSharpModulith.Host;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodosInfrastructureMarker).Assembly);
    }
}
