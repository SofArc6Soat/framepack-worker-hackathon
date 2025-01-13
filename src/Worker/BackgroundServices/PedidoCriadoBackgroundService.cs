using Amazon.DynamoDBv2.DataModel;
using Core.Infra.MessageBroker;
using Infra.Dto;
using Worker.Dtos.Events;

namespace Worker.BackgroundServices
{
    public class PedidoCriadoBackgroundService(ISqsService<PedidoCriadoEvent> sqsClient, IServiceScopeFactory serviceScopeFactory, ILogger<PedidoCriadoBackgroundService> logger) : BackgroundService
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

        private async Task ProcessMessageAsync(PedidoCriadoEvent? message, CancellationToken cancellationToken)
        {
            if (message is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();

                var pedidoExistente = await repository.LoadAsync<PedidoDb>(message.Id, cancellationToken);

                if (pedidoExistente is null)
                {
                    await repository.SaveAsync(ConvertMessageToDb(message), cancellationToken);
                }
            }
        }

        private static PedidoDb ConvertMessageToDb(PedidoCriadoEvent message) =>
            new()
            {
                Id = message.Id,
                NumeroPedido = message.NumeroPedido,
                ValorTotal = message.ValorTotal,
                DataPedido = message.DataPedido
            };
    }
}
