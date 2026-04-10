namespace App.Capability.Todos;

public class TodosException : Exception
{
    public TodosException()
    {
    }

    public TodosException(string message)
        : base(message)
    {
    }

    public TodosException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
