namespace Core.Infra.EmailSender
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia um e-mail de forma assíncrona.
        /// </summary>
        /// <param name="recipientEmail">O endereço de e-mail do destinatário.</param>
        /// <param name="subject">O assunto do e-mail.</param>
        /// <param name="bodyText">O corpo do e-mail em texto simples.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        Task SendEmailAsync(string recipientEmail, string subject, string bodyText);
    }
}
