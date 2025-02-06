using Controllers;
using Core.Infra.MessageBroker;
using Gateways.Dtos.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Worker.BackgroundServices;

namespace SmokeTests.SmokeTests;

public class DownloadEfetuadoBackgroundServiceTests
{
    private readonly Mock<ISqsService<DownloadEfetuadoEvent>> _sqsClientMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<ILogger<DownloadEfetuadoBackgroundService>> _loggerMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConversaoController> _controllerMock;

    public DownloadEfetuadoBackgroundServiceTests()
    {
        _sqsClientMock = new Mock<ISqsService<DownloadEfetuadoEvent>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<DownloadEfetuadoBackgroundService>>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _controllerMock = new Mock<IConversaoController>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConversaoController))).Returns(_controllerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessMessageSuccessfully()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var downloadEfetuadoEvent = new DownloadEfetuadoEvent { Id = Guid.NewGuid(), UrlArquivoVideo = "http://example.com/video.mp4" };

        _sqsClientMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(downloadEfetuadoEvent);
        _controllerMock.Setup(x => x.ProcessarDownloadEfetuadoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new DownloadEfetuadoBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        var executeTask = service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100); // Aguarde um pouco para permitir a execução do loop
        cancellationTokenSource.Cancel();
        await executeTask;

        // Assert
        _sqsClientMock.Verify(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _controllerMock.Verify(x => x.ProcessarDownloadEfetuadoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogErrorWhenExceptionOccurs()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var exception = new Exception("Test exception");

        _sqsClientMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(exception);

        var service = new DownloadEfetuadoBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        var executeTask = service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100); // Aguarde um pouco para permitir a execução do loop
        cancellationTokenSource.Cancel();
        await executeTask;

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing messages.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldNotProcessNullMessage()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var service = new DownloadEfetuadoBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        await service.ProcessMessageAsync(null, cancellationToken);

        // Assert
        _controllerMock.Verify(x => x.ProcessarDownloadEfetuadoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
