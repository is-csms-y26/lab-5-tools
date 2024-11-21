using Lab5.Tools.Application.Contracts.Orders;
using Lab5.Tools.Application.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Tools.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderService, OrderService>();

        return collection;
    }
}