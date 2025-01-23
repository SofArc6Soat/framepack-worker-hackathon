using Amazon.SQS;
using Core.Infra.MessageBroker;
using Core.Infra.MessageBroker.DependencyInjection;
using Gateways.Dtos.Events;
using System.Diagnostics.CodeAnalysis;
using Worker.BackgroundServices;

namespace Worker.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddWorkerDependencyServices(this IServiceCollection services, WorkerQueues queues)
        {
            // AWS SQS
            services.AddAwsSqsMessageBroker();

            services.AddSingleton<ISqsService<ConversaoSolicitadaEvent>>(provider => new SqsService<ConversaoSolicitadaEvent>(provider.GetRequiredService<IAmazonSQS>(), queues.QueueConversaoSolicitadaEvent));

            services.AddHostedService<ConversaoSolicitadaBackgroundService>();
        }
    }

    [ExcludeFromCodeCoverage]
    public record WorkerQueues
    {
        public string QueueConversaoSolicitadaEvent { get; set; } = string.Empty;
    }
}