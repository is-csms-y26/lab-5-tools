using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Contracts.Orders.Events;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka.ProducerHandlers.Orders;

internal class OrderPackingStartedHandler : IEventHandler<OrderPackingStartedEvent>
{
    private readonly IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderPackingStartedHandler(IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderPackingStartedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            PackingStarted = new OrderProcessingValue.Types.OrderPackingStarted
            {
                OrderId = evt.OrderId,
                PackingBy = evt.PackingBy,
                StartedAt = evt.StartedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}