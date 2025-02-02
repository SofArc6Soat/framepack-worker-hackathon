using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Core.Infra.EmailSender
{
    public class EmailService(IAmazonSimpleEmailService sesClient, string senderEmail) : IEmailService
    {
        public async Task SendEmailAsync(string recipientEmail, string subject, string bodyText)
        {
            var destination = new Destination
            {
                ToAddresses = [recipientEmail]
            };

            var message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content { Charset = "UTF-8", Data = bodyText }
                }
            };

            var sendRequest = new SendEmailRequest
            {
                Source = senderEmail,
                Destination = destination,
                Message = message
            };

            try
            {
                var response = await sesClient.SendEmailAsync(sendRequest);
                Console.WriteLine($"E-mail enviado com sucesso! Message ID: {response.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                throw;
            }
        }
    }
}
