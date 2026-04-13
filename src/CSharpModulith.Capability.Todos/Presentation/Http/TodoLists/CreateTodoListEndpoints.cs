using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class CreateTodoListEndpoints
{
    public static void MapCreateTodoList(this RouteGroupBuilder group)
    {
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
    }

    private sealed record CreateTodoListBody(string Title);
}
