using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Contracts.Orders.Events;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka.ProducerHandlers.Orders;

internal class OrderDeliveryStartedHandler : IEventHandler<OrderDeliveryStartedEvent>
{
    private readonly IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderDeliveryStartedHandler(IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderDeliveryStartedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            DeliveryStarted = new OrderProcessingValue.Types.OrderDeliveryStarted
            {
                OrderId = evt.OrderId,
                DeliveredBy = evt.DeliveredBy,
                StartedAt = evt.StartedBy.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}