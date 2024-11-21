using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Abstractions.Persistence.Repositories;

namespace Lab5.Tools.Infrastructure.Persistence;

internal class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IOrderRepository orders)
    {
        Orders = orders;
    }

    public IOrderRepository Orders { get; }
}