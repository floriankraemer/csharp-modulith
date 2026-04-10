namespace App.Shared;

class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}