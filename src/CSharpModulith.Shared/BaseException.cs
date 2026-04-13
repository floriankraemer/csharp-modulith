namespace App.Shared;

public class BaseException : Exception
{
    public BaseException(string message)
        : base(message)
    {
    }

    public static BaseException WithMessage(string message) =>
        new BaseException(message);
}
