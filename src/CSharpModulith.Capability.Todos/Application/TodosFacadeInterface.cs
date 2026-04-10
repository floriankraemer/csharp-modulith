using App.Capability.Todos.Application.Requests;
using App.Capability.Todos.Application.Responses;

namespace App.Capability.Todos.Application;

/// <summary>
/// Specification:
///
/// - Exposes todo list creation, listing, item mutation, and read access for other capabilities and presentation.
/// - Maps use-case outcomes to primitive request/response DTOs without catching domain exceptions.
/// </summary>
public interface TodosFacadeInterface
{
    Task<CreateTodoListResponse> CreateTodoList(
        CreateTodoListRequest request,
        CancellationToken cancellationToken = default);

    Task<AddTodoItemResponse> AddTodoItem(
        AddTodoItemRequest request,
        CancellationToken cancellationToken = default);

    Task<SetTodoItemCompletedResponse> SetTodoItemCompleted(
        SetTodoItemCompletedRequest request,
        CancellationToken cancellationToken = default);

    Task<GetTodoListResponse> GetTodoList(
        GetTodoListRequest request,
        CancellationToken cancellationToken = default);

    Task<ListTodoListsResponse> ListTodoLists(
        ListTodoListsRequest request,
        CancellationToken cancellationToken = default);
}
