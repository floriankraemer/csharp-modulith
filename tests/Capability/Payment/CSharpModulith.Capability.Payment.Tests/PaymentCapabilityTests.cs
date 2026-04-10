using App.Capability.Payment.Domain;

namespace App.Capability.Payment.Tests;

[Trait("Capability", "Payment")]
public sealed class PaymentCapabilityTests
{
    [Fact]
    public void Domain_marker_type_is_reachable()
    {
        _ = typeof(PaymentDomainMarker);
    }
}
