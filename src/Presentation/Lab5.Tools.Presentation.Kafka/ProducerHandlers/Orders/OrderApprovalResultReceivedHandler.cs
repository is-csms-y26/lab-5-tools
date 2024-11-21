using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Contracts.Orders.Events;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka.ProducerHandlers.Orders;

internal class OrderApprovalResultReceivedHandler : IEventHandler<OrderApprovalResultReceivedEvent>
{
    private readonly IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderApprovalResultReceivedHandler(IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderApprovalResultReceivedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            ApprovalReceived = new OrderProcessingValue.Types.OrderApprovalReceived
            {
                OrderId = evt.OrderId,
                IsApproved = evt.IsApproved,
                CreatedBy = evt.CreatedBy,
                CreatedAt = evt.CreatedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}