using App.Capability.Todos.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wolverine.EntityFrameworkCore;

namespace CSharpModulith.Host;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IOptions<AppEnvelopePersistenceOptions> envelopePersistenceOptions) : DbContext(options)
{
    private readonly AppEnvelopePersistenceOptions _envelopePersistenceOptions = envelopePersistenceOptions.Value;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (_envelopePersistenceOptions.MapWolverineEnvelopeStorage)
        {
            modelBuilder.MapWolverineEnvelopeStorage();
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodosInfrastructureMarker).Assembly);
    }
}
