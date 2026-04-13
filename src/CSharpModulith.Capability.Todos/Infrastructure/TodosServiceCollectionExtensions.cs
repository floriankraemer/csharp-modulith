using App.Capability.Todos.Application;
using App.Capability.Todos.Application.DomainEventHandlers;
using App.Capability.Todos.Application.UseCases.AddTodoItem;
using App.Capability.Todos.Application.UseCases.CreateTodoList;
using App.Capability.Todos.Application.Repositories;
using App.Capability.Todos.Application.UseCases.GetTodoList;
using App.Capability.Todos.Application.UseCases.ListTodoLists;
using App.Capability.Todos.Application.UseCases.SetTodoItemCompleted;
using App.Capability.Todos.Domain.Model.TodoList.Events;
using App.Capability.Todos.Domain.Repositories;
using App.Capability.Todos.Infrastructure.Persistence.EfCore;
using App.Shared.Application;
using Microsoft.Extensions.DependencyInjection;

namespace App.Capability.Todos.Infrastructure;

public static class TodosServiceCollectionExtensions
{
    public static IServiceCollection AddTodosCapability(this IServiceCollection services)
    {
        services.AddScoped<TodoListPersistenceMapper>();
        services.AddScoped<TodoListWriteRepositoryInterface, TodoListWriteRepository>();
        services.AddScoped<TodoListReadRepositoryInterface, TodoListReadRepository>();
        services.AddScoped<EventHandlerInterface<TodoListWasCreated>, TodoListWasCreatedHandler>();
        services.AddScoped<EventHandlerInterface<TodoItemWasAdded>, TodoItemWasAddedHandler>();
        services.AddScoped<CreateTodoList>();
        services.AddScoped<AddTodoItem>();
        services.AddScoped<SetTodoItemCompleted>();
        services.AddScoped<GetTodoList>();
        services.AddScoped<ListTodoLists>();
        services.AddScoped<TodosFacadeInterface, TodosFacade>();
        return services;
    }
}
