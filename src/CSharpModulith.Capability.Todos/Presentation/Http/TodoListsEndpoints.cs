using System.Text.Json;
using App.Capability.Todos;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http;

public static class TodoListsEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static IEndpointRouteBuilder MapTodoListsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todo-lists");

        group.MapGet(
            string.Empty,
            async (TodosFacadeInterface facade, CancellationToken ct) =>
            {
                var response = await facade.ListTodoLists(new ListTodoListsRequest(), ct);

                if (!response.IsSuccessful)
                {
                    return Results.BadRequest(new { error = response.Failure.ToString(), message = response.Message });
                }

                var todoLists = JsonSerializer.Deserialize<List<TodoListListItemPayload>>(
                    response.TodoListsJson!,
                    JsonOptions);

                return Results.Ok(new { data = new { todoLists } });
            });

        group.MapPost(
            string.Empty,
            async (CreateTodoListBody body, TodosFacadeInterface facade, CancellationToken ct) =>
            {
                var response = await facade.CreateTodoList(
                    new CreateTodoListRequest(Title: body.Title),
                    ct);

                return response.IsSuccessful
                    ? Results.Created($"/api/todo-lists/{response.ListId}", new { data = new { listId = response.ListId } })
                    : Results.BadRequest(new { error = response.Failure.ToString(), message = response.Message });
            });

        group.MapGet(
            "{listId:guid}",
            async (Guid listId, TodosFacadeInterface facade, CancellationToken ct) =>
            {
                var response = await facade.GetTodoList(
                    new GetTodoListRequest(ListId: listId.ToString()),
                    ct);

                if (!response.IsSuccessful)
                {
                    return response.Failure == TodosFailure.ListNotFound
                        ? Results.NotFound()
                        : Results.BadRequest(new { error = response.Failure.ToString(), message = response.Message });
                }

                return Results.Ok(new
                {
                    data = new
                    {
                        list = new
                        {
                            listId = response.ListId,
                            title = response.Title,
                            itemsJson = response.ItemsJson,
                        },
                    },
                });
            });

        group.MapPost(
            "{listId:guid}/items",
            async (Guid listId, AddTodoItemBody body, TodosFacadeInterface facade, CancellationToken ct) =>
            {
                var response = await facade.AddTodoItem(
                    new AddTodoItemRequest(ListId: listId.ToString(), Title: body.Title),
                    ct);

                if (!response.IsSuccessful)
                {
                    return response.Failure == TodosFailure.ListNotFound
                        ? Results.NotFound()
                        : Results.BadRequest(new { error = response.Failure.ToString(), message = response.Message });
                }

                return Results.Ok(new { data = new { itemId = response.ItemId } });
            });

        group.MapPatch(
            "{listId:guid}/items/{itemId:guid}/completed",
            async (Guid listId, Guid itemId, SetCompletedBody body, TodosFacadeInterface facade, CancellationToken ct) =>
            {
                var response = await facade.SetTodoItemCompleted(
                    new SetTodoItemCompletedRequest(
                        ListId: listId.ToString(),
                        ItemId: itemId.ToString(),
                        Completed: body.Completed),
                    ct);

                if (!response.IsSuccessful)
                {
                    if (response.Failure == TodosFailure.ListNotFound || response.Failure == TodosFailure.ItemNotFound)
                    {
                        return Results.NotFound();
                    }

                    return Results.BadRequest(new { error = response.Failure.ToString(), message = response.Message });
                }

                return Results.NoContent();
            });

        return app;
    }

    private sealed record CreateTodoListBody(string Title);

    private sealed record AddTodoItemBody(string Title);

    private sealed record SetCompletedBody(bool Completed);

    private sealed record TodoListListItemPayload(string ListId, string Title);
}
