namespace App.Capability.Todos.Domain.Model.TodoList;

public readonly record struct TodoListId(Guid Value)
{
    public static TodoListId From(Guid value) => new(value);

    public static TodoListId FromString(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
