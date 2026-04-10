namespace App.Capability.Todos.Domain.Model.TodoList;

public readonly record struct TodoItemId(Guid Value)
{
    public static TodoItemId From(Guid value) => new(value);

    public static TodoItemId FromString(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
