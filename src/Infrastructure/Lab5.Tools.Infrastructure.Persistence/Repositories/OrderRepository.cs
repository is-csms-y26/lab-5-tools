using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Abstractions.Persistence.Repositories;
using Lab5.Tools.Application.Models.Orders;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab5.Tools.Infrastructure.Persistence.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public OrderRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<Order> QueryAsync(
        OrderQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        select  order_id,
                order_state, 
                order_created_at, 
                order_updated_at
        from orders
        where 
            (cardinality(:ids) = 0 or order_id = any (:ids))
            and order_id > :cursor
        limit :page_size;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.OrderIds)
            .AddParameter("cursor", query.Cursor)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Order(
                Id: reader.GetInt64("order_id"),
                State: reader.GetFieldValue<OrderState>("order_state"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("order_created_at"),
                UpdatedAt: reader.GetFieldValue<DateTimeOffset>("order_updated_at"));
        }
    }

    public async Task AddOrUpdateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into orders (order_id, order_state, order_created_at, order_updated_at)
        select * from unnest(:ids, :states, :created_at, :updated_at)
        on conflict on constraint orders_pkey
        do update 
        set order_state = excluded.order_state,
            order_created_at = excluded.order_created_at,
            order_updated_at = excluded.order_updated_at;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", orders.Select(x => x.Id))
            .AddParameter("states", orders.Select(x => x.State))
            .AddParameter("created_at", orders.Select(x => x.CreatedAt))
            .AddParameter("updated_at", orders.Select(x => x.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}