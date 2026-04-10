using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace App.Capability.Todos.Tests.TestInfrastructure;

public static class TestDbContextFactory
{
    /// <summary>
    /// Creates an open SQLite in-memory database and a context that shares its connection.
    /// </summary>
    public static (SqliteConnection Connection, TodosTestDbContext Context) CreateSqliteInMemory()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TodosTestDbContext(options);
        context.Database.EnsureCreated();

        return (connection, context);
    }
}
