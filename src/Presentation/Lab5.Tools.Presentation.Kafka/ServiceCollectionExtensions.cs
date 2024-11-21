using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Lab5.Tools.Presentation.Kafka.ConsumerHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Kafka.Contracts;

namespace Lab5.Tools.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        const string producerKey = "Presentation:Kafka:Producers";

        collection.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(consumer => consumer
                .WithKey<OrderCreationKey>()
                .WithValue<OrderCreationValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:OrderCreation"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleInboxWith<OrderCreationConsumerHandler>())
            .AddProducer(producer => producer
                .WithKey<OrderProcessingKey>()
                .WithValue<OrderProcessingValue>()
                .WithConfiguration(configuration.GetSection($"{producerKey}:OrderProcessing"))
                .SerializeKeyWithProto()
                .SerializeValueWithProto()
                .WithOutbox()));

        return collection;
    }

    public static IEventsConfigurationBuilder AddPresentationKafkaEventHandlers(
        this IEventsConfigurationBuilder builder)
    {
        return builder.AddHandlersFromAssemblyContaining<IAssemblyMarker>();
    }
}