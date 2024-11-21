using Itmo.Dev.Platform.Events;

namespace Lab5.Tools.Application.Contracts.Orders.Events;

public record OrderDeliveryStartedEvent(long OrderId, string DeliveredBy, DateTimeOffset StartedBy) : IEvent;