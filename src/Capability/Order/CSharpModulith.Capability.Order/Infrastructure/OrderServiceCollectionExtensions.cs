using Microsoft.Extensions.DependencyInjection;

namespace App.Capability.Order.Infrastructure;

public static class OrderServiceCollectionExtensions
{
    public static IServiceCollection AddOrderCapability(this IServiceCollection services)
    {
        return services;
    }
}
