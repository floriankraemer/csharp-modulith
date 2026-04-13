using App.Capability.Todos;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class GetTodoListEndpoints
{
    public static void MapGetTodoList(this RouteGroupBuilder group)
    {
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
    }
}
