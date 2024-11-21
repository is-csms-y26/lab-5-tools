using Lab5.Tools.Application.Contracts.Orders.Operations;

namespace Lab5.Tools.Application.Contracts.Orders;

public interface IOrderService
{
    Task CreateAsync(CreateOrder.Request request, CancellationToken cancellationToken);

    Task<StartOrderProcessing.Result> StartProcessingAsync(
        StartOrderProcessing.Request request,
        CancellationToken cancellationToken);

    Task<ApproveOrder.Result> ApproveAsync(ApproveOrder.Request request, CancellationToken cancellationToken);

    Task<StartOrderPacking.Result> StartPackingAsync(
        StartOrderPacking.Request request,
        CancellationToken cancellationToken);

    Task<FinishOrderPacking.Result> FinishPackingAsync(
        FinishOrderPacking.Request request,
        CancellationToken cancellationToken);

    Task<StartOrderDelivery.Result> StartDeliveryAsync(
        StartOrderDelivery.Request request,
        CancellationToken cancellationToken);

    Task<FinishOrderDelivery.Result> FinishDeliveryAsync(
        FinishOrderDelivery.Request request,
        CancellationToken cancellationToken);
}