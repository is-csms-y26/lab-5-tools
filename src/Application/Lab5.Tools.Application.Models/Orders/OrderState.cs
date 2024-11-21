namespace Lab5.Tools.Application.Models.Orders;

public enum OrderState
{
    Created,
    PendingApproval,
    Approved,
    Packing,
    Packed,
    InDelivery,
    Delivered,
    Cancelled,
}