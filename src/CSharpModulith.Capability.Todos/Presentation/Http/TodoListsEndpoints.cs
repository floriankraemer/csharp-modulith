using App.Capability.Todos.Presentation.Http.TodoLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace App.Capability.Todos.Presentation.Http;

public static class TodoListsEndpoints
{
    public static IEndpointRouteBuilder MapTodoListsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todo-lists");
        group.MapListTodoLists();
        group.MapCreateTodoList();
        group.MapGetTodoList();
        group.MapAddTodoItem();
        group.MapSetTodoItemCompleted();

        return app;
    }
}
