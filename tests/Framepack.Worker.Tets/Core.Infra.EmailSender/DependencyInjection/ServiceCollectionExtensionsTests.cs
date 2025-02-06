using Amazon.SimpleEmail;
using Core.Infra.EmailSender;
using Core.Infra.EmailSender.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Framepack.Worker.Tets.Core.Infra.EmailSender.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEmailSender_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddEmailSender());
    }

    [Fact]
    public void AddEmailSender_ShouldRegisterIAmazonSimpleEmailService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEmailSender();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var sesClient = serviceProvider.GetService<IAmazonSimpleEmailService>();
        Assert.NotNull(sesClient);
    }

    [Fact]
    public void AddEmailSender_ShouldRegisterIEmailService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEmailSender();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var emailService = serviceProvider.GetService<IEmailService>();
        Assert.NotNull(emailService);
    }

    [Fact]
    public void AddEmailSender_ShouldThrowException_WhenSenderEmailIsNullOrEmpty()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockSesClient = new Mock<IAmazonSimpleEmailService>();
        services.AddSingleton(mockSesClient.Object);

        // Act
        services.AddSingleton<IEmailService>(sp =>
        {
            var sesClient = sp.GetRequiredService<IAmazonSimpleEmailService>();
            var senderEmail = string.Empty; // Simulating empty sender email

            return string.IsNullOrEmpty(senderEmail)
                ? throw new Exception("O senderEmail não foi configurado corretamente em EmailSettings:SenderEmail")
                : new EmailService(sesClient, senderEmail);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.Throws<Exception>(() => serviceProvider.GetService<IEmailService>());
    }
}