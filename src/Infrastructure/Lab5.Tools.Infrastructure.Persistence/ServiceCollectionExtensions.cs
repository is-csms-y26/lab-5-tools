using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Abstractions.Persistence.Repositories;
using Lab5.Tools.Infrastructure.Persistence.Plugins;
using Lab5.Tools.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Tools.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(builder => builder.BindConfiguration("Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)
                .WithDataSourcePlugin<MappingPlugin>()));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IOrderRepository, OrderRepository>();

        return collection;
    }
}