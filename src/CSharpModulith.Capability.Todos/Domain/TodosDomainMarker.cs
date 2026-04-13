namespace App.Capability.Todos.Domain;

/// <summary>
/// Empty anchor type so architecture tests can load this assembly via
/// <c>typeof(TodosDomainMarker).Assembly</c> (see <c>CapabilityLayerArchitectureTests</c> in
/// <c>CSharpModulith.Architecture.Tests</c>). Not used by application code.
/// </summary>
public static class TodosDomainMarker
{
}
