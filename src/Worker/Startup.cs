using Controllers.DependencyInjection;
using Core.WebApi.DependencyInjection;
using Gateways.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Worker.Configuration;
using Worker.DependencyInjection;
using static Gateways.DependencyInjection.ServiceCollectionExtensions;

namespace Worker
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = EnvironmentConfig.ConfigureEnvironment(services, _configuration);

            services.AddWorkerDefautConfig();

            services.AddHealthCheckConfig();

            services.AddControllerDependencyServices();

            var queues = new Queues
            {
                QueueConversaoSolicitadaEvent = settings.AwsSqsSettings.QueueConversaoSolicitadaEvent,
                QueueDownloadEfetuadoEvent = settings.AwsSqsSettings.QueueDownloadEfetuadoEvent
            };

            services.AddGatewayDependencyServices(settings.AwsDynamoDbSettings.ServiceUrl, settings.AwsDynamoDbSettings.AccessKey, settings.AwsDynamoDbSettings.SecretKey, queues);

            var workerQueues = new WorkerQueues
            {
                QueueConversaoSolicitadaEvent = settings.AwsSqsSettings.QueueConversaoSolicitadaEvent,
                QueueDownloadEfetuadoEvent = settings.AwsSqsSettings.QueueDownloadEfetuadoEvent
            };

            services.AddWorkerDependencyServices(workerQueues);
        }

        public static void Configure(IApplicationBuilder app) =>
            app.UseWorkerDefautConfig();
    }
}