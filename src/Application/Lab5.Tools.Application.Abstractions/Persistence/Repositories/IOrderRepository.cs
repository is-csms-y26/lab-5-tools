using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Models.Orders;

namespace Lab5.Tools.Application.Abstractions.Persistence.Repositories;

public interface IOrderRepository
{
    IAsyncEnumerable<Order> QueryAsync(OrderQuery query, CancellationToken cancellationToken);

    Task AddOrUpdateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken);
}