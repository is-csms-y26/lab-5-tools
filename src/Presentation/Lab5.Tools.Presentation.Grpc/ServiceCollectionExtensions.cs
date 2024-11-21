using Itmo.Dev.Platform.Grpc.Services;
using Lab5.Tools.Presentation.Grpc.Controller;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Tools.Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection collection)
    {
        collection.AddPlatformGrpcServices(builder => builder);
        return collection;
    }

    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<OrderController>();
            endpoints.MapGrpcReflectionService();
        });

        return builder;
    }
}