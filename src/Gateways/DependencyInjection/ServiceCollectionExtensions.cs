using Amazon.SQS;
using Core.Infra.MessageBroker;
using Core.Infra.S3.DependencyInjection;
using Gateways.Dtos.Events;
using Gateways.Handlers;
using Infra.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Gateways.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddGatewayDependencyServices(this IServiceCollection services, string dynamoDbServiceUrl, string dynamoDbAccessKey, string dynamoDbSecretKey, Queues queues)
        {
            services.AddScoped<IConversaoGateway, ConversaoGateway>();
            services.AddScoped<IArquivoHandler, ArquivoHandler>();
            services.AddScoped<IVideoHandler, VideoHandler>();

            services.AddAwsS3();

            services.AddInfraDependencyServices(dynamoDbServiceUrl, dynamoDbAccessKey, dynamoDbSecretKey);

            services.AddSingleton<ISqsService<ConversaoSolicitadaEvent>>(provider => new SqsService<ConversaoSolicitadaEvent>(provider.GetRequiredService<IAmazonSQS>(), queues.QueueConversaoSolicitadaEvent));
        }

        [ExcludeFromCodeCoverage]
        public record Queues
        {
            public string QueueConversaoSolicitadaEvent { get; set; } = string.Empty;
        }
    }
}