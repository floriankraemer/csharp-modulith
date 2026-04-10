using App.Capability.Todos;

namespace App.Capability.Todos.Application;

public sealed class TodosApplicationException : TodosException
{
    private TodosApplicationException(string message)
        : base(message)
    {
    }

    public static TodosApplicationException ForValidation(string message) => new(message);
}
