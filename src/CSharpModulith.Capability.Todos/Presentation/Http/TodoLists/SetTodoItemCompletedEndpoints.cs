using App.Capability.Todos;
using App.Capability.Todos.Application;
using App.Capability.Todos.Application.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class SetTodoItemCompletedEndpoints
{
    public static void MapSetTodoItemCompleted(this RouteGroupBuilder group)
    {
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
    }

    private sealed record SetCompletedBody(bool Completed);
}
