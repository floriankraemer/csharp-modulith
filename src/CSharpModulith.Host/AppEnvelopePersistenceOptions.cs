namespace CSharpModulith.Host;

/// <summary>
/// When true, EF maps Wolverine envelope tables for transactional outbox (PostgreSQL + Wolverine persistence).
/// </summary>
public sealed class AppEnvelopePersistenceOptions
{
    public bool MapWolverineEnvelopeStorage { get; set; }
}
