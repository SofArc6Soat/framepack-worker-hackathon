using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace Core.Infra.MessageBroker
{
    public interface ISqsService<T>
    {
        Task<bool> SendMessageAsync(T objeto);
        Task<T?> ReceiveMessagesAsync(CancellationToken cancellationToken);
    }

    public class SqsService<T>(IAmazonSQS sqsClient, string queueUrl) : ISqsService<T>
    {
        public async Task<bool> SendMessageAsync(T objeto)
        {
            var messageBody = JsonSerializer.Serialize(objeto);
            var sendRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            var response = await sqsClient.SendMessageAsync(sendRequest);

            if (response is not null)
            {
                Console.WriteLine($"Objeto enviado com sucesso. ID: {response.MessageId}");
                return true;
            }

            return false;
        }

        public async Task<T?> ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 10
            };

            var response = await sqsClient.ReceiveMessageAsync(receiveRequest);

            foreach (var message in response.Messages)
            {
                try
                {
                    var jsonOptions = GetOptions();

                    var objeto = JsonSerializer.Deserialize<T>(message.Body);

                    await sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);

                    Console.WriteLine($"Objeto recebido: {JsonSerializer.Serialize(objeto, jsonOptions)}");

                    return objeto;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Erro ao receber objeto: {ex.Message}");
                }
            }

            return default;
        }

        private static JsonSerializerOptions GetOptions() => new() { WriteIndented = true };
    }
}