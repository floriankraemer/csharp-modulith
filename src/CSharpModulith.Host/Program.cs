using App.Capability.Order.Infrastructure;
using App.Capability.Payment.Infrastructure;
using App.Capability.Todos.Infrastructure;
using App.Capability.Todos.Presentation.Http;
using App.Shared;
using Aspire.RabbitMQ.Client;
using CSharpModulith.Host;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var postgresConnectionString = builder.Configuration.GetConnectionString("postgresdb");
if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    var sqliteConnectionString = builder.Configuration.GetConnectionString("sqlite")
        ?? "Data Source=modulith-dev.db";
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
}
else
{
    builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");
}

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("sqlite")))
{
    builder.AddSqliteConnection(name: "sqlite");
}

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("messaging")))
{
    builder.AddRabbitMQClient(connectionName: "messaging");
}

builder.Services.AddSingleton(new AppConfig(builder.Environment.EnvironmentName));
builder.Services.AddOrderCapability();
builder.Services.AddPaymentCapability();
builder.Services.AddTodosCapability();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (IsSqliteDatabase(db))
    {
        db.Database.EnsureCreated();
    }
    else if (app.Environment.IsDevelopment())
    {
        db.Database.Migrate();
    }
}

static bool IsSqliteDatabase(AppDbContext db) =>
    db.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/alive", new HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });
}

app.MapGet("/", () => Results.Ok(new { name = "CSharpModulith" }));

app.MapTodoListsEndpoints();

app.Run();
