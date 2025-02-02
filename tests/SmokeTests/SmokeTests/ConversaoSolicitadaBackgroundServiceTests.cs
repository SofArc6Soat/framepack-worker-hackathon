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
    private readonly Mock<ISqsService<ConversaoSolicitadaEvent>> _sqsServiceMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<ILogger<ConversaoSolicitadaBackgroundService>> _loggerMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConversaoController> _conversaoControllerMock;
    private readonly ConversaoSolicitadaBackgroundService _backgroundService;

    public ConversaoSolicitadaBackgroundServiceTests()
    {
        _sqsServiceMock = new Mock<ISqsService<ConversaoSolicitadaEvent>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<ConversaoSolicitadaBackgroundService>>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _conversaoControllerMock = new Mock<IConversaoController>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConversaoController))).Returns(_conversaoControllerMock.Object);

        _backgroundService = new ConversaoSolicitadaBackgroundService(_sqsServiceMock.Object, _serviceScopeFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SmokeTest_ShouldStartAndProcessMessage()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var testEvent = new ConversaoSolicitadaEvent { Id = Guid.NewGuid() };

        _sqsServiceMock.Setup(s => s.ReceiveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(testEvent);
        _conversaoControllerMock.Setup(c => c.ProcessarConversaoSolicitadaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var executeTask = _backgroundService.StartAsync(cancellationTokenSource.Token);

        // Allow some time for the worker to process
        await Task.Delay(1000);

        // Cancel the worker
        cancellationTokenSource.Cancel();
        await _backgroundService.StopAsync(cancellationTokenSource.Token);

        // Assert
        _sqsServiceMock.Verify(s => s.ReceiveMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _conversaoControllerMock.Verify(c => c.ProcessarConversaoSolicitadaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
