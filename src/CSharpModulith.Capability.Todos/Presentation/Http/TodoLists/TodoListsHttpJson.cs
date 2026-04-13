using System.Text.Json;

namespace App.Capability.Todos.Presentation.Http.TodoLists;

internal static class TodoListsHttpJson
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
