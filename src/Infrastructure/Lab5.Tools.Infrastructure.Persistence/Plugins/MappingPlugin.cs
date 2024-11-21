using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Lab5.Tools.Application.Models.Orders;
using Npgsql;

namespace Lab5.Tools.Infrastructure.Persistence.Plugins;

internal class MappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<OrderState>("order_state");
    }
}