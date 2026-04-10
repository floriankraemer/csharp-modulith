using App.Capability.Todos;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using App.Capability.Todos.Infrastructure;
using App.Capability.Todos.Tests.TestInfrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Capability.Todos.Tests;

[Trait("Capability", "Todos")]
public sealed class TodosFacadeTests
{
    [Fact]
    public async Task createTodoList_returns_list_id_when_title_valid()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var ctx = new TodosTestDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(options);
        services.AddScoped<TodosTestDbContext>(_ => new TodosTestDbContext(options));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TodosTestDbContext>());
        services.AddTodosCapability();
        await using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<TodosFacadeInterface>();

        // Act
        var response = await facade.CreateTodoList(new CreateTodoListRequest(Title: "Books"));

        // Assert
        Assert.True(response.IsSuccessful);
        Assert.NotNull(response.ListId);
        Assert.True(Guid.TryParse(response.ListId, out _));
    }

    [Fact]
    public async Task createTodoList_returns_validation_failure_when_title_empty()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var ctx = new TodosTestDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(options);
        services.AddScoped<TodosTestDbContext>(_ => new TodosTestDbContext(options));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TodosTestDbContext>());
        services.AddTodosCapability();
        await using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<TodosFacadeInterface>();

        // Act
        var response = await facade.CreateTodoList(new CreateTodoListRequest(Title: "   "));

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Equal(TodosFailure.ValidationError, response.Failure);
    }

    [Fact]
    public async Task setTodoItemCompleted_returns_item_not_found_when_item_missing()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var ctx = new TodosTestDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(options);
        services.AddScoped<TodosTestDbContext>(_ => new TodosTestDbContext(options));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TodosTestDbContext>());
        services.AddTodosCapability();
        await using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<TodosFacadeInterface>();

        var created = await facade.CreateTodoList(new CreateTodoListRequest(Title: "List"));
        var listId = created.ListId!;

        // Act
        var response = await facade.SetTodoItemCompleted(
            new SetTodoItemCompletedRequest(
                ListId: listId,
                ItemId: Guid.NewGuid().ToString(),
                Completed: true));

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Equal(TodosFailure.ItemNotFound, response.Failure);
    }

    [Fact]
    public async Task listTodoLists_returns_empty_todo_lists_json_when_none_exist()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var ctx = new TodosTestDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(options);
        services.AddScoped<TodosTestDbContext>(_ => new TodosTestDbContext(options));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TodosTestDbContext>());
        services.AddTodosCapability();
        await using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<TodosFacadeInterface>();

        // Act
        var response = await facade.ListTodoLists(new ListTodoListsRequest());

        // Assert
        Assert.True(response.IsSuccessful);
        Assert.Equal("[]", response.TodoListsJson);
    }

    [Fact]
    public async Task listTodoLists_returns_json_for_all_lists()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<TodosTestDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var ctx = new TodosTestDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(options);
        services.AddScoped<TodosTestDbContext>(_ => new TodosTestDbContext(options));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TodosTestDbContext>());
        services.AddTodosCapability();
        await using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<TodosFacadeInterface>();

        await facade.CreateTodoList(new CreateTodoListRequest(Title: "First"));
        await facade.CreateTodoList(new CreateTodoListRequest(Title: "Second"));

        // Act
        var response = await facade.ListTodoLists(new ListTodoListsRequest());

        // Assert
        Assert.True(response.IsSuccessful);
        Assert.Contains("First", response.TodoListsJson, StringComparison.Ordinal);
        Assert.Contains("Second", response.TodoListsJson, StringComparison.Ordinal);
    }
}
