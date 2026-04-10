namespace App.Capability.Todos.Domain;

public sealed class InvariantViolationException : TodosDomainException
{
    private InvariantViolationException(string message)
        : base(message)
    {
    }

    public static InvariantViolationException Because(string message) => new(message);
}
