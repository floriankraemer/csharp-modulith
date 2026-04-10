using App.Capability.Todos;

namespace App.Capability.Todos.Infrastructure;

public sealed class TodosInfrastructureException : TodosException
{
    private TodosInfrastructureException(string message)
        : base(message)
    {
    }

    private TodosInfrastructureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static TodosInfrastructureException Because(string message) => new(message);

    public static TodosInfrastructureException Because(string message, Exception inner) =>
        new(message, inner);
}
