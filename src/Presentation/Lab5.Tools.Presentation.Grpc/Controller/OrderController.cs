using Grpc.Core;
using Lab5.Tools.Application.Contracts.Orders;
using Orders.ProcessingService.Contracts;
using Lab5.Tools.Application.Contracts.Orders.Operations;
using System.Diagnostics;

namespace Lab5.Tools.Presentation.Grpc.Controller;

public class OrderController : OrderService.OrderServiceBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    public override async Task<ApproveOrderResponse> ApproveOrder(
        ApproveOrderRequest request,
        ServerCallContext context)
    {
        var applicationRequest = new ApproveOrder.Request(
            request.OrderId,
            request.IsApproved,
            request.ApprovedBy,
            request.FailureReason);

        ApproveOrder.Result response = await _service.ApproveAsync(
            applicationRequest,
            context.CancellationToken);

        return response switch
        {
            ApproveOrder.Result.Success _ => new ApproveOrderResponse(),

            ApproveOrder.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"Cannot approve order in state = {invalidState.State}")),

            ApproveOrder.Result.OrderNotFound _ => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Order not found")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<StartOrderPackingResponse> StartOrderPacking(
        StartOrderPackingRequest request,
        ServerCallContext context)
    {
        var applicationRequest = new StartOrderPacking.Request(
            request.OrderId,
            request.PackingBy);

        StartOrderPacking.Result response = await _service.StartPackingAsync(
            applicationRequest,
            context.CancellationToken);

        return response switch
        {
            StartOrderPacking.Result.Success _ => new StartOrderPackingResponse(),

            StartOrderPacking.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"Cannot approve order in state = {invalidState.State}")),

            StartOrderPacking.Result.OrderNotFound _ => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Order not found")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<FinishOrderPackingResponse> FinishOrderPacking(
        FinishOrderPackingRequest request,
        ServerCallContext context)
    {
        var applicationRequest = new FinishOrderPacking.Request(
            request.OrderId,
            request.IsSuccessful,
            request.FailureReason);

        FinishOrderPacking.Result response = await _service.FinishPackingAsync(
            applicationRequest,
            context.CancellationToken);

        return response switch
        {
            FinishOrderPacking.Result.Success _ => new FinishOrderPackingResponse(),

            FinishOrderPacking.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"Cannot approve order in state = {invalidState.State}")),

            FinishOrderPacking.Result.OrderNotFound _ => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Order not found")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<StartOrderDeliveryResponse> StartOrderDelivery(
        StartOrderDeliveryRequest request,
        ServerCallContext context)
    {
        var applicationRequest = new StartOrderDelivery.Request(
            request.OrderId,
            request.DeliveredBy);

        StartOrderDelivery.Result response = await _service.StartDeliveryAsync(
            applicationRequest,
            context.CancellationToken);

        return response switch
        {
            StartOrderDelivery.Result.Success _ => new StartOrderDeliveryResponse(),

            StartOrderDelivery.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"Cannot approve order in state = {invalidState.State}")),

            StartOrderDelivery.Result.OrderNotFound _ => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Order not found")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<FinishOrderPackingResponse> FinishOrderDelivery(
        FinishOrderPackingRequest request,
        ServerCallContext context)
    {
        var applicationRequest = new FinishOrderDelivery.Request(
            request.OrderId,
            request.IsSuccessful,
            request.FailureReason);

        FinishOrderDelivery.Result response = await _service.FinishDeliveryAsync(
            applicationRequest,
            context.CancellationToken);

        return response switch
        {
            FinishOrderDelivery.Result.Success _ => new FinishOrderPackingResponse(),

            FinishOrderDelivery.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                $"Cannot approve order in state = {invalidState.State}")),

            FinishOrderDelivery.Result.OrderNotFound _ => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Order not found")),

            _ => throw new UnreachableException(),
        };
    }
}