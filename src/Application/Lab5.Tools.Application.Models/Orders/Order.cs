namespace Lab5.Tools.Application.Models.Orders;

public record Order(long Id, OrderState State, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);