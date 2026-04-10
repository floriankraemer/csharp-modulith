using App.Capability.Order.Domain;
using App.Capability.Payment.Domain;
using App.Capability.Todos.Domain;
using App.Shared;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitArchitecture = ArchUnitNET.Domain.Architecture;
using ArchUnitNET.Domain;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace CSharpModulith.Architecture.Tests;

/// <summary>
/// ArchUnitNET analyzes compiled assemblies; the library authors recommend running these tests in Debug.
/// </summary>
public sealed class CapabilityLayerArchitectureTests
{
    private static readonly ArchUnitArchitecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(AppConfig).Assembly,
            typeof(OrderDomainMarker).Assembly,
            typeof(PaymentDomainMarker).Assembly,
            typeof(TodosDomainMarker).Assembly)
        .Build();

    private static readonly IObjectProvider<IType> CapabilityDomainLayer = Types()
        .That()
        .ResideInNamespace("App.Capability.Order.Domain")
        .Or()
        .ResideInNamespace("App.Capability.Payment.Domain")
        .Or()
        .ResideInNamespace("App.Capability.Todos.Domain")
        .As("Capability Domain layers");

    private static readonly IObjectProvider<IType> CapabilityApplicationLayer = Types()
        .That()
        .ResideInNamespace("App.Capability.Order.Application")
        .Or()
        .ResideInNamespace("App.Capability.Payment.Application")
        .Or()
        .ResideInNamespace("App.Capability.Todos.Application")
        .As("Capability Application layers");

    private static readonly IObjectProvider<IType> CapabilityInfrastructureLayer = Types()
        .That()
        .ResideInNamespace("App.Capability.Order.Infrastructure")
        .Or()
        .ResideInNamespace("App.Capability.Payment.Infrastructure")
        .Or()
        .ResideInNamespace("App.Capability.Todos.Infrastructure")
        .As("Capability Infrastructure layers");

    private static readonly IObjectProvider<IType> CapabilityPresentationLayer = Types()
        .That()
        .ResideInNamespace("App.Capability.Order.Presentation")
        .Or()
        .ResideInNamespace("App.Capability.Payment.Presentation")
        .Or()
        .ResideInNamespace("App.Capability.Todos.Presentation")
        .As("Capability Presentation layers");

    private static readonly IObjectProvider<IType> SharedKernelTypes = Types()
        .That()
        .ResideInNamespace("App.Shared")
        .As("App.Shared kernel");

    private static readonly IObjectProvider<IType> AspNetCoreTypes = Types()
        .That()
        .ResideInNamespace("Microsoft.AspNetCore")
        .As("Microsoft.AspNetCore");

    private static readonly IObjectProvider<IType> CapabilityNamespaces = Types()
        .That()
        .ResideInNamespace("App.Capability")
        .As("App.Capability namespaces");

    [Fact]
    public void domain_layers_do_not_depend_on_application_infrastructure_or_presentation()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityDomainLayer)
            .Should()
            .NotDependOnAny(CapabilityApplicationLayer)
            .Because("Domain must not reference Application");
        AssertArchitecture(rule);

        rule = Types()
            .That()
            .Are(CapabilityDomainLayer)
            .Should()
            .NotDependOnAny(CapabilityInfrastructureLayer)
            .Because("Domain must not reference Infrastructure");
        AssertArchitecture(rule);

        rule = Types()
            .That()
            .Are(CapabilityDomainLayer)
            .Should()
            .NotDependOnAny(CapabilityPresentationLayer)
            .Because("Domain must not reference Presentation");
        AssertArchitecture(rule);
    }

    [Fact]
    public void application_layers_do_not_depend_on_infrastructure_or_presentation()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityApplicationLayer)
            .Should()
            .NotDependOnAny(CapabilityInfrastructureLayer)
            .Because("Application must not reference Infrastructure");
        AssertArchitecture(rule);

        rule = Types()
            .That()
            .Are(CapabilityApplicationLayer)
            .Should()
            .NotDependOnAny(CapabilityPresentationLayer)
            .Because("Application must not reference Presentation");
        AssertArchitecture(rule);
    }

    [Fact]
    public void infrastructure_layers_do_not_depend_on_presentation()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityInfrastructureLayer)
            .Should()
            .NotDependOnAny(CapabilityPresentationLayer)
            .Because("Infrastructure must not reference Presentation");
        AssertArchitecture(rule);
    }

    [Fact]
    public void presentation_layers_do_not_depend_on_infrastructure()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityPresentationLayer)
            .Should()
            .NotDependOnAny(CapabilityInfrastructureLayer)
            .Because("Presentation must not reference Infrastructure");
        AssertArchitecture(rule);
    }

    [Fact]
    public void domain_layers_do_not_depend_on_microsoft_aspnetcore()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityDomainLayer)
            .Should()
            .NotDependOnAny(AspNetCoreTypes)
            .Because("Domain must not reference ASP.NET Core types");
        AssertArchitecture(rule);
    }

    [Fact]
    public void application_layers_do_not_depend_on_microsoft_aspnetcore()
    {
        IArchRule rule = Types()
            .That()
            .Are(CapabilityApplicationLayer)
            .Should()
            .NotDependOnAny(AspNetCoreTypes)
            .Because("Application must not reference ASP.NET Core types");
        AssertArchitecture(rule);
    }

    [Fact]
    public void shared_kernel_does_not_depend_on_microsoft_aspnetcore()
    {
        IArchRule rule = Types()
            .That()
            .Are(SharedKernelTypes)
            .Should()
            .NotDependOnAny(AspNetCoreTypes)
            .Because("App.Shared must not reference ASP.NET Core types");
        AssertArchitecture(rule);
    }

    [Fact]
    public void shared_kernel_does_not_depend_on_capability_namespaces()
    {
        IArchRule rule = Types()
            .That()
            .Are(SharedKernelTypes)
            .Should()
            .NotDependOnAny(CapabilityNamespaces)
            .Because("App.Shared must not reference capability modules");
        AssertArchitecture(rule);
    }

    private static void AssertArchitecture(IArchRule rule)
    {
        Assert.True(
            rule.HasNoViolations(Architecture),
            $"Architecture rule failed: {rule}");
    }
}
