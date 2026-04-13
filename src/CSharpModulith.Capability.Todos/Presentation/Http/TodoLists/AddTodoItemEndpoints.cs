using App.Capability.Todos;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class AddTodoItemEndpoints
{
    public static void MapAddTodoItem(this RouteGroupBuilder group)
    {
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
    }

    private sealed record AddTodoItemBody(string Title);
}
