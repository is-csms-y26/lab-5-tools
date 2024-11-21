using Lab5.Tools.Application.Abstractions.Persistence.Repositories;

namespace Lab5.Tools.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IOrderRepository Orders { get; }
}