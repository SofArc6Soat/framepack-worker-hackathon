using Controllers;
using Core.Infra.MessageBroker;
using Gateways.Dtos.Events;

namespace Worker.BackgroundServices
{
    public class DownloadEfetuadoBackgroundService(ISqsService<DownloadEfetuadoEvent> sqsClient, IServiceScopeFactory serviceScopeFactory, ILogger<DownloadEfetuadoBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessageAsync(await sqsClient.ReceiveMessagesAsync(stoppingToken), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while processing messages.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        public async Task ProcessMessageAsync(DownloadEfetuadoEvent? message, CancellationToken cancellationToken)
        {
            if (message is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var controller = scope.ServiceProvider.GetRequiredService<IConversaoController>();

                await controller.ProcessarDownloadEfetuadoAsync(message.Id, cancellationToken);
            }
        }
    }
}
