using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.MessagePersistence.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Observability;
using Lab5.Tools.Application;
using Lab5.Tools.Infrastructure.Persistence;
using Lab5.Tools.Presentation.Grpc;
using Lab5.Tools.Presentation.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatform();
builder.AddPlatformObservability();
builder.Services.AddUtcDateTimeProvider();

// Null value ignore is needed to correctly deserialize oneof messages in inbox/outbox
builder.Services.AddOptions<JsonSerializerSettings>()
    .Configure(options => options.NullValueHandling = NullValueHandling.Ignore);

builder.Services.AddSingleton<JsonSerializerSettings>(
    provider => provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

// Used as inbox and outbox infrastructure
builder.Services.AddPlatformMessagePersistence(selector => selector
    .UsePostgresPersistence(postgres => postgres
        .ConfigureOptions(optionsBuilder => optionsBuilder
            .BindConfiguration("Infrastructure:MessagePersistence:Persistence"))));

// Used as backbone to publishing kafka messages from application layer without explicit or inversed dependencies
builder.Services.AddPlatformEvents(events => events
    .AddPresentationKafkaEventHandlers());

builder.Services
    .AddApplication()
    .AddInfrastructurePersistence()
    .AddPresentationGrpc()
    .AddPresentationKafka(builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();
app.UsePresentationGrpc();

app.UsePlatformObservability();

await app.RunAsync();