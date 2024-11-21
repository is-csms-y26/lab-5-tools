using Itmo.Dev.Platform.Events;

namespace Lab5.Tools.Application.Contracts.Orders.Events;

public record OrderPackingStartedEvent(long OrderId, string PackingBy, DateTimeOffset StartedAt) : IEvent;