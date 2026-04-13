using System.Text.Json;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class ListTodoListsEndpoints
{
    public static void MapListTodoLists(this RouteGroupBuilder group)
    {
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
                    TodoListsHttpJson.JsonOptions);

                return Results.Ok(new { data = new { todoLists } });
            });
    }

    private sealed record TodoListListItemPayload(string ListId, string Title);
}
