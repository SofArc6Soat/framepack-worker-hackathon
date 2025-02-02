using Amazon;
using Amazon.SimpleEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infra.EmailSender.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailSender(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<IAmazonSimpleEmailService>(sp =>
                new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1));

            services.AddSingleton<IEmailService>(sp =>
            {
                var sesClient = sp.GetRequiredService<IAmazonSimpleEmailService>();
                var senderEmail = "sof.arc.6soat@gmail.com";

                return string.IsNullOrEmpty(senderEmail)
                    ? throw new("O senderEmail não foi configurado corretamente em EmailSettings:SenderEmail")
                    : new EmailService(sesClient, senderEmail);
            });
        }
    }
}