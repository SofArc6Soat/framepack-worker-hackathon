using Core.WebApi.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Worker.Configuration;

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

            //services.AddControllerDependencyServices();

            //var queues = new Queues
            //{
            //    QueuePedidoPagoEvent = settings.AwsSqsSettings.QueuePedidoPagoEvent,
            //    QueuePedidoPendentePagamentoEvent = settings.AwsSqsSettings.QueuePedidoPendentePagamentoEvent
            //};

            //services.AddGatewayDependencyServices(settings.AwsDynamoDbSettings.ServiceUrl, settings.AwsDynamoDbSettings.AccessKey, settings.AwsDynamoDbSettings.SecretKey, queues);

            //var workerQueues = new WorkerQueues
            //{
            //    QueuePedidoCriadoEvent = settings.AwsSqsSettings.QueuePedidoCriadoEvent
            //};

            //services.AddWorkerDependencyServices(workerQueues);
        }

        public static void Configure(IApplicationBuilder app) =>
            app.UseWorkerDefautConfig();
    }
}