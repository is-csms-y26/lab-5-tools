using Itmo.Dev.Platform.Kafka.Consumer;
using Itmo.Dev.Platform.MessagePersistence;
using Lab5.Tools.Application.Contracts.Orders;
using Lab5.Tools.Application.Contracts.Orders.Operations;
using Microsoft.Extensions.Logging;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka.ConsumerHandlers;

internal class OrderCreationConsumerHandler : IKafkaInboxHandler<OrderCreationKey, OrderCreationValue>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderCreationConsumerHandler> _logger;

    public OrderCreationConsumerHandler(IOrderService orderService, ILogger<OrderCreationConsumerHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<OrderCreationKey, OrderCreationValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<OrderCreationKey, OrderCreationValue> message in messages)
        {
            if (message.Value.EventCase is OrderCreationValue.EventOneofCase.OrderCreated)
            {
                var request = new CreateOrder.Request(
                    message.Value.OrderCreated.OrderId,
                    message.Value.OrderCreated.CreatedAt.ToDateTimeOffset());

                await _orderService.CreateAsync(request, cancellationToken);
            }
            else if (message.Value.EventCase is OrderCreationValue.EventOneofCase.OrderProcessingStarted)
            {
                var request = new StartOrderProcessing.Request(
                    message.Value.OrderProcessingStarted.OrderId,
                    message.Value.OrderProcessingStarted.StartedAt.ToDateTimeOffset());

                StartOrderProcessing.Result result = await _orderService
                    .StartProcessingAsync(request, cancellationToken);

                if (result is not StartOrderProcessing.Result.Success)
                {
                    message.SetResult(MessageHandleResult.Failure);
                }
            }
            else
            {
                _logger.LogError(
                    "Invalid event case received = {EventCase} for order = {OrderId}",
                    message.Value.EventCase,
                    message.Key.OrderId);

                message.SetResult(MessageHandleResult.Failure);
            }
        }
    }
}