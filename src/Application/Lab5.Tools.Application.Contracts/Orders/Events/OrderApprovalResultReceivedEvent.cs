using Itmo.Dev.Platform.Events;

namespace Lab5.Tools.Application.Contracts.Orders.Events;

public record OrderApprovalResultReceivedEvent(
    long OrderId,
    bool IsApproved,
    string CreatedBy,
    DateTimeOffset CreatedAt) : IEvent;