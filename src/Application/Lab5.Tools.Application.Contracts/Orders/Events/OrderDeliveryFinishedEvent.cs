using Itmo.Dev.Platform.Events;

namespace Lab5.Tools.Application.Contracts.Orders.Events;

public record OrderDeliveryFinishedEvent(
    long OrderId,
    DateTimeOffset FinishedAt,
    bool IsSuccessful,
    string? FailureReason) : IEvent;