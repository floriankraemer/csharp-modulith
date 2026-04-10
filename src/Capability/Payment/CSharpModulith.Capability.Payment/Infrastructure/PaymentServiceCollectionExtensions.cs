using Microsoft.Extensions.DependencyInjection;

namespace App.Capability.Payment.Infrastructure;

public static class PaymentServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentCapability(this IServiceCollection services)
    {
        return services;
    }
}
