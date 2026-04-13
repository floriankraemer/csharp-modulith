using App.Capability.Todos.Infrastructure.Message;
using App.Shared.Infrastructure.Persistence;
using App.Shared.Infrastructure.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace CSharpModulith.Host;

internal static class WolverineHostExtensions
{
    public static WebApplicationBuilder AddModulithPersistenceAndWolverine(this WebApplicationBuilder builder)
    {
        var postgresConnectionString = builder.Configuration.GetConnectionString("postgresdb");

        builder.Services.Configure<AppEnvelopePersistenceOptions>(options =>
        {
            options.MapWolverineEnvelopeStorage = !string.IsNullOrWhiteSpace(postgresConnectionString);
        });

        builder.Host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(TodosDomainEventWolverineBridge).Assembly);

            if (!string.IsNullOrWhiteSpace(postgresConnectionString))
            {
                opts.PersistMessagesWithPostgresql(
                    postgresConnectionString,
                    "public");
                opts.UseEntityFrameworkCoreTransactions();
                opts.Services.AddDbContextWithWolverineIntegration<AppDbContext>(
                    (serviceProvider, db) =>
                    {
                        _ = db.UseNpgsql(postgresConnectionString)
                            .AddInterceptors(
                                serviceProvider.GetRequiredService<PostSaveDomainEventsSaveChangesInterceptor>());
                    });
            }

            if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("messaging")))
            {
                opts.UseRabbitMqUsingNamedConnection("messaging");
            }
        });

        if (string.IsNullOrWhiteSpace(postgresConnectionString))
        {
            var sqliteConnectionString = builder.Configuration.GetConnectionString("sqlite")
                ?? "Data Source=modulith-dev.db";
            builder.Services.AddDbContextPool<AppDbContext>(
                (serviceProvider, options) =>
                {
                    options.UseSqlite(sqliteConnectionString)
                        .AddInterceptors(
                            serviceProvider.GetRequiredService<PostSaveDomainEventsSaveChangesInterceptor>());
                });
            builder.Services.AddScoped<
                DomainEventPersistenceCoordinatorInterface,
                SaveChangesOnlyDomainEventPersistenceCoordinator>();
        }

        if (!string.IsNullOrWhiteSpace(postgresConnectionString))
        {
            builder.Services.AddScoped<
                DomainEventPersistenceCoordinatorInterface,
                WolverineDomainEventPersistenceCoordinator>();
        }

        return builder;
    }
}
