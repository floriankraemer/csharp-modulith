using App.Capability.Order.Domain;

namespace App.Capability.Order.Tests;

[Trait("Capability", "Order")]
public sealed class OrderCapabilityTests
{
    [Fact]
    public void Domain_marker_type_is_reachable()
    {
        _ = typeof(OrderDomainMarker);
    }
}
