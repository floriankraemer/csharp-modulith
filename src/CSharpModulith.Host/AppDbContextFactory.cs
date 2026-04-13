using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace CSharpModulith.Host;

/// <summary>
/// Design-time factory for EF Core migrations (provides <see cref="AppEnvelopePersistenceOptions"/> for Wolverine envelope mapping).
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        _ = optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=modulith_design;Username=postgres;Password=postgres");
        var envelopeOptions = Options.Create(
            new AppEnvelopePersistenceOptions { MapWolverineEnvelopeStorage = true });

        return new AppDbContext(optionsBuilder.Options, envelopeOptions);
    }
}
