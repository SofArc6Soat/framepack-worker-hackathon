using Amazon.SQS;
using Core.Infra.MessageBroker;
using Core.Infra.MessageBroker.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Worker.BackgroundServices;
using Worker.Dtos.Events;

namespace Worker.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddWorkerDependencyServices(this IServiceCollection services, WorkerQueues queues)
        {
            // AWS SQS
            services.AddAwsSqsMessageBroker();

            services.AddSingleton<ISqsService<PedidoCriadoEvent>>(provider => new SqsService<PedidoCriadoEvent>(provider.GetRequiredService<IAmazonSQS>(), queues.QueuePedidoCriadoEvent));

            services.AddHostedService<PedidoCriadoBackgroundService>();
        }
    }

    [ExcludeFromCodeCoverage]
    public record WorkerQueues
    {
        public string QueuePedidoCriadoEvent { get; set; } = string.Empty;
    }
}