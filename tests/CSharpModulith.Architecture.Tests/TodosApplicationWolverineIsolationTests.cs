using App.Capability.Todos.Domain;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitArchitecture = ArchUnitNET.Domain.Architecture;
using Wolverine;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace CSharpModulith.Architecture.Tests;

/// <summary>
/// Ensures the Todos Application layer stays free of Wolverine types (composition uses adapters only).
/// </summary>
public sealed class TodosApplicationWolverineIsolationTests
{
    private static readonly ArchUnitArchitecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(TodosDomainMarker).Assembly,
            typeof(IMessageBus).Assembly)
        .Build();

    [Fact]
    public void todos_application_types_do_not_depend_on_wolverine()
    {
        IObjectProvider<IType> todosApplication = Types()
            .That()
            .ResideInNamespace("App.Capability.Todos.Application")
            .As("Todos Application layer");

        IObjectProvider<IType> wolverine = Types()
            .That()
            .ResideInAssembly(typeof(IMessageBus).Assembly)
            .As("Wolverine");

        IArchRule rule = Types()
            .That()
            .Are(todosApplication)
            .Should()
            .NotDependOnAny(wolverine)
            .Because("Application must use owned ports (e.g. EventHandlerInterface), not Wolverine APIs");

        Assert.True(
            rule.HasNoViolations(Architecture),
            $"Architecture rule failed: {rule}");
    }
}
