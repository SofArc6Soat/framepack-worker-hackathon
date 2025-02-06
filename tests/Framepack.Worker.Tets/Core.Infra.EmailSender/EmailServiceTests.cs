using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Core.Infra.EmailSender;
using Moq;

namespace Framepack.Worker.Tets.Core.Infra.EmailSender;

public class EmailServiceTests
{
    private readonly Mock<IAmazonSimpleEmailService> _sesClientMock;
    private readonly EmailService _emailService;
    private readonly string _senderEmail = "sender@example.com";

    public EmailServiceTests()
    {
        _sesClientMock = new Mock<IAmazonSimpleEmailService>();
        _emailService = new EmailService(_sesClientMock.Object, _senderEmail);
    }

    [Fact]
    public async Task SendEmailAsync_Success()
    {
        // Arrange
        var recipientEmail = "recipient@example.com";
        var subject = "Test Subject";
        var bodyText = "Test Body";
        var sendEmailResponse = new SendEmailResponse { MessageId = "test-message-id" };

        _sesClientMock
            .Setup(s => s.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ReturnsAsync(sendEmailResponse);

        // Act
        await _emailService.SendEmailAsync(recipientEmail, subject, bodyText);

        // Assert
        _sesClientMock.Verify(s => s.SendEmailAsync(It.Is<SendEmailRequest>(req =>
            req.Source == _senderEmail &&
            req.Destination.ToAddresses.Contains(recipientEmail) &&
            req.Message.Subject.Data == subject &&
            req.Message.Body.Text.Data == bodyText
        ), default), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_Error()
    {
        // Arrange
        var recipientEmail = "recipient@example.com";
        var subject = "Test Subject";
        var bodyText = "Test Body";
        var exceptionMessage = "Error sending email";

        _sesClientMock
            .Setup(s => s.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _emailService.SendEmailAsync(recipientEmail, subject, bodyText));
        Assert.Equal(exceptionMessage, exception.Message);
    }
}