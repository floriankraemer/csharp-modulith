using App.Capability.Todos;

namespace App.Capability.Todos.Domain;

public class TodosDomainException : TodosException
{
    protected TodosDomainException(string message)
        : base(message)
    {
    }
}
