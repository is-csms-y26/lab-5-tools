using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Contracts.Orders;
using Lab5.Tools.Application.Contracts.Orders.Events;
using Lab5.Tools.Application.Contracts.Orders.Operations;
using Lab5.Tools.Application.Models.Orders;
using System.Data;

namespace Lab5.Tools.Application.Orders;

internal class OrderService : IOrderService
{
    private readonly IPersistenceContext _context;
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEventPublisher _eventPublisher;

    public OrderService(
        IPersistenceContext context,
        IPersistenceTransactionProvider transactionProvider,
        IDateTimeProvider dateTimeProvider,
        IEventPublisher eventPublisher)
    {
        _context = context;
        _transactionProvider = transactionProvider;
        _dateTimeProvider = dateTimeProvider;
        _eventPublisher = eventPublisher;
    }

    public async Task CreateAsync(CreateOrder.Request request, CancellationToken cancellationToken)
    {
        var order = new Order(request.OrderId, OrderState.Created, request.CreatedAt, request.CreatedAt);
        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
    }

    public async Task<StartOrderProcessing.Result> StartProcessingAsync(
        StartOrderProcessing.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderProcessing.Result.OrderNotFound();

        order = order with
        {
            State = OrderState.PendingApproval,
            UpdatedAt = request.StartedAt,
        };

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);

        return new StartOrderProcessing.Result.Success();
    }

    public async Task<ApproveOrder.Result> ApproveAsync(
        ApproveOrder.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new ApproveOrder.Result.OrderNotFound();

        if (order.State is not OrderState.PendingApproval)
            return new ApproveOrder.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsApproved ? OrderState.Approved : OrderState.Cancelled,
            UpdatedAt = _dateTimeProvider.Current,
        };

        var evt = new OrderApprovalResultReceivedEvent(
            request.OrderId,
            request.IsApproved,
            request.CreatedBy,
            order.UpdatedAt);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new ApproveOrder.Result.Success();
    }

    public async Task<StartOrderPacking.Result> StartPackingAsync(
        StartOrderPacking.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderPacking.Result.OrderNotFound();

        if (order.State is not OrderState.Approved)
            return new StartOrderPacking.Result.InvalidState(order.State);

        order = order with
        {
            State = OrderState.Packing,
            UpdatedAt = _dateTimeProvider.Current,
        };

        var evt = new OrderPackingStartedEvent(order.Id, request.PackingBy, order.UpdatedAt);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new StartOrderPacking.Result.Success();
    }

    public async Task<FinishOrderPacking.Result> FinishPackingAsync(
        FinishOrderPacking.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new FinishOrderPacking.Result.OrderNotFound();

        if (order.State is not OrderState.Packing)
            return new FinishOrderPacking.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsSuccessful ? OrderState.Packed : OrderState.Cancelled,
            UpdatedAt = _dateTimeProvider.Current,
        };

        var evt = new OrderPackingFinishedEvent(
            order.Id,
            order.UpdatedAt,
            request.IsSuccessful,
            request.FailureReason);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new FinishOrderPacking.Result.Success();
    }

    public async Task<StartOrderDelivery.Result> StartDeliveryAsync(
        StartOrderDelivery.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderDelivery.Result.OrderNotFound();

        if (order.State is not OrderState.Packed)
            return new StartOrderDelivery.Result.InvalidState(order.State);

        order = order with
        {
            State = OrderState.InDelivery,
            UpdatedAt = _dateTimeProvider.Current,
        };

        var evt = new OrderDeliveryStartedEvent(
            order.Id,
            request.DeliveredBy,
            order.UpdatedAt);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new StartOrderDelivery.Result.Success();
    }

    public async Task<FinishOrderDelivery.Result> FinishDeliveryAsync(
        FinishOrderDelivery.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = OrderQuery.Build(builder => builder
            .WithOrderId(request.OrderId)
            .WithPageSize(1));

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new FinishOrderDelivery.Result.OrderNotFound();

        if (order.State is not OrderState.InDelivery)
            return new FinishOrderDelivery.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsSuccessful ? OrderState.Delivered : OrderState.Cancelled,
            UpdatedAt = _dateTimeProvider.Current,
        };

        var evt = new OrderDeliveryFinishedEvent(
            order.Id,
            order.UpdatedAt,
            request.IsSuccessful,
            request.FailureReason);

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new FinishOrderDelivery.Result.Success();
    }
}