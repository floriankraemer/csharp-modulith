namespace App.Shared;

/// <summary>
/// Shared application configuration; modules compose this via constructor injection (no inheritance).
/// </summary>
public sealed class AppConfig(string environment)
{
    public string Environment() => environment;
}
