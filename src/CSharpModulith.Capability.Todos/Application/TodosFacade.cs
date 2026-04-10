using App.Capability.Todos.Application.Requests;
using App.Capability.Todos.Application.Responses;
using App.Capability.Todos.Application.UseCases.AddTodoItem;
using App.Capability.Todos.Application.UseCases.CreateTodoList;
using App.Capability.Todos.Application.UseCases.GetTodoList;
using App.Capability.Todos.Application.UseCases.ListTodoLists;
using App.Capability.Todos.Application.UseCases.SetTodoItemCompleted;

namespace App.Capability.Todos.Application;

public sealed class TodosFacade(
    CreateTodoList createTodoList,
    AddTodoItem addTodoItem,
    SetTodoItemCompleted setTodoItemCompleted,
    GetTodoList getTodoList,
    ListTodoLists listTodoLists) : TodosFacadeInterface
{
    public async Task<CreateTodoListResponse> CreateTodoList(
        CreateTodoListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await createTodoList.ExecuteAsync(
            new CreateTodoListInput(Title: request.Title),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return CreateTodoListResponse.WithFailure(
                failure: result.Failure!.Value,
                message: result.Message);
        }

        return CreateTodoListResponse.WithSuccess(listId: result.ListId!);
    }

    public async Task<AddTodoItemResponse> AddTodoItem(
        AddTodoItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await addTodoItem.ExecuteAsync(
            new AddTodoItemInput(ListId: request.ListId, Title: request.Title),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return AddTodoItemResponse.WithFailure(
                failure: result.Failure!.Value,
                message: result.Message);
        }

        return AddTodoItemResponse.WithSuccess(itemId: result.ItemId!);
    }

    public async Task<SetTodoItemCompletedResponse> SetTodoItemCompleted(
        SetTodoItemCompletedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await setTodoItemCompleted.ExecuteAsync(
            new SetTodoItemCompletedInput(
                ListId: request.ListId,
                ItemId: request.ItemId,
                Completed: request.Completed),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return SetTodoItemCompletedResponse.WithFailure(
                failure: result.Failure!.Value,
                message: result.Message);
        }

        return SetTodoItemCompletedResponse.WithSuccess();
    }

    public async Task<GetTodoListResponse> GetTodoList(
        GetTodoListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await getTodoList.ExecuteAsync(
            new GetTodoListInput(ListId: request.ListId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return GetTodoListResponse.WithFailure(
                failure: result.Failure!.Value,
                message: result.Message);
        }

        return GetTodoListResponse.WithSuccess(
            listId: result.ListId!,
            title: result.Title!,
            itemsJson: result.ItemsJson!);
    }

    public async Task<ListTodoListsResponse> ListTodoLists(
        ListTodoListsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await listTodoLists.ExecuteAsync(
            new ListTodoListsInput(),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return ListTodoListsResponse.WithFailure(
                failure: result.Failure!.Value,
                message: result.Message);
        }

        return ListTodoListsResponse.WithSuccess(todoListsJson: result.TodoListsJson!);
    }
}
