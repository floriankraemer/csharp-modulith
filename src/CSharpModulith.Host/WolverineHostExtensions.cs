using App.Capability.Todos.Infrastructure.Message;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace CSharpModulith.Host;

internal static class WolverineHostExtensions
{
    public static WebApplicationBuilder AddModulithWolverine(this WebApplicationBuilder builder)
    {
        builder.Host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(TodosDomainEventWolverineBridge).Assembly);

            var postgresConnectionString = builder.Configuration.GetConnectionString("postgresdb");
            if (!string.IsNullOrWhiteSpace(postgresConnectionString))
            {
                opts.PersistMessagesWithPostgresql(
                    postgresConnectionString,
                    "public");
                opts.UseEntityFrameworkCoreTransactions();
            }

            if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("messaging")))
            {
                opts.UseRabbitMqUsingNamedConnection("messaging");
            }
        });

        return builder;
    }
}
