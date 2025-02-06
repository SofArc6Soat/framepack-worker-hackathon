using Controllers;
using Core.Infra.MessageBroker;
using Gateways.Dtos.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Worker.BackgroundServices;

namespace SmokeTests.SmokeTests;

public class ConversaoSolicitadaBackgroundServiceTests
{
    private readonly Mock<ISqsService<ConversaoSolicitadaEvent>> _sqsClientMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<ILogger<ConversaoSolicitadaBackgroundService>> _loggerMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConversaoController> _conversaoControllerMock;

    public ConversaoSolicitadaBackgroundServiceTests()
    {
        _sqsClientMock = new Mock<ISqsService<ConversaoSolicitadaEvent>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<ConversaoSolicitadaBackgroundService>>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _conversaoControllerMock = new Mock<IConversaoController>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConversaoController))).Returns(_conversaoControllerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessMessages()
    {
        // Arrange
        var stoppingToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;
        var service = new ConversaoSolicitadaBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        _sqsClientMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConversaoSolicitadaEvent { Id = Guid.NewGuid() });

        _conversaoControllerMock.Setup(x => x.ProcessarConversaoSolicitadaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Corrigido para retornar Task<bool>

        // Act
        await service.StartAsync(stoppingToken);
        await Task.Delay(2000); // Espera para garantir que o loop de execução seja iniciado
        await service.StopAsync(stoppingToken);

        // Assert
        _sqsClientMock.Verify(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _conversaoControllerMock.Verify(x => x.ProcessarConversaoSolicitadaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var stoppingToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;
        var service = new ConversaoSolicitadaBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        _sqsClientMock.Setup(x => x.ReceiveMessagesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        await service.StartAsync(stoppingToken);
        await Task.Delay(2000); // Espera para garantir que o loop de execução seja iniciado
        await service.StopAsync(stoppingToken);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing messages.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldCallController_WhenMessageIsNotNull()
    {
        // Arrange
        var message = new ConversaoSolicitadaEvent { Id = Guid.NewGuid() };
        var cancellationToken = new CancellationToken();
        var service = new ConversaoSolicitadaBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        await service.ProcessMessageAsync(message, cancellationToken);

        // Assert
        _conversaoControllerMock.Verify(x => x.ProcessarConversaoSolicitadaAsync(message.Id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldNotCallController_WhenMessageIsNull()
    {
        // Arrange
        ConversaoSolicitadaEvent? message = null;
        var cancellationToken = new CancellationToken();
        var service = new ConversaoSolicitadaBackgroundService(_sqsClientMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);

        // Act
        await service.ProcessMessageAsync(message, cancellationToken);

        // Assert
        _conversaoControllerMock.Verify(x => x.ProcessarConversaoSolicitadaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
