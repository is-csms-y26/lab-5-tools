using Lab5.Tools.Application.Models.Orders;

namespace Lab5.Tools.Application.Contracts.Orders.Operations;

public static class FinishOrderPacking
{
    public readonly record struct Request(long OrderId, bool IsSuccessful, string? FailureReason);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record OrderNotFound : Result;

        public sealed record InvalidState(OrderState State) : Result;
    }
}