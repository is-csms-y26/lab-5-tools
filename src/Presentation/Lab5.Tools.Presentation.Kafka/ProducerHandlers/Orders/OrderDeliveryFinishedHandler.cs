using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Contracts.Orders.Events;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka.ProducerHandlers.Orders;

public class OrderDeliveryFinishedHandler : IEventHandler<OrderDeliveryFinishedEvent>
{
    private readonly IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderDeliveryFinishedHandler(IKafkaMessageProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderDeliveryFinishedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            DeliveryFinished = new OrderProcessingValue.Types.OrderDeliveryFinished
            {
                OrderId = evt.OrderId,
                FinishedAt = evt.FinishedAt.ToTimestamp(),
                IsFinishedSuccessfully = evt.IsSuccessful,
                FailureReason = evt.FailureReason,
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}