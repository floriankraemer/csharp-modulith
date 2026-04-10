using App.Capability.Todos;

namespace App.Capability.Todos.Presentation;

public sealed class TodosPresentationException : TodosException
{
    private TodosPresentationException(string message)
        : base(message)
    {
    }

    public static TodosPresentationException Because(string message) => new(message);
}
