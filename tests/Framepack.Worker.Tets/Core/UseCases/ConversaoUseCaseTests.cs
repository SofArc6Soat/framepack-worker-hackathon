using Core.Domain.Notificacoes;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways;
using Moq;
using UseCases;

namespace Framepack.Worker.Tets.Core.UseCases;

public class ConversaoUseCaseTests
{
    private readonly Mock<IConversaoGateway> _conversaoGatewayMock;
    private readonly Mock<INotificador> _notificadorMock;
    private readonly ConversaoUseCase _conversaoUseCase;

    public ConversaoUseCaseTests()
    {
        _conversaoGatewayMock = new Mock<IConversaoGateway>();
        _notificadorMock = new Mock<INotificador>();
        _conversaoUseCase = new ConversaoUseCase(_conversaoGatewayMock.Object, _notificadorMock.Object);
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_ConversaoNaoEncontrada_DeveLancarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        _conversaoGatewayMock.Setup(g => g.ObterConversaoAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversao)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _conversaoUseCase.ProcessarConversaoSolicitadaAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_ConversaoEncontrada_DeveEfetuarConversao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversao = new Conversao(id, "usuarioId", DateTime.Now, Status.AguardandoConversao, "nomeArquivo", "urlArquivoVideo");
        _conversaoGatewayMock.Setup(g => g.ObterConversaoAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversao);
        _conversaoGatewayMock.Setup(g => g.EfetuarConversaoAsync(conversao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _conversaoUseCase.ProcessarConversaoSolicitadaAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result);
        _conversaoGatewayMock.Verify(g => g.EfetuarConversaoAsync(conversao, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_ConversaoEncontrada_ErroAoEfetuarConversao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversao = new Conversao(id, "usuarioId", DateTime.Now, Status.AguardandoConversao, "nomeArquivo", "urlArquivoVideo");
        _conversaoGatewayMock.Setup(g => g.ObterConversaoAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversao);
        _conversaoGatewayMock.Setup(g => g.EfetuarConversaoAsync(conversao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _conversaoUseCase.ProcessarConversaoSolicitadaAsync(id, CancellationToken.None);

        // Assert
        Assert.False(result);
        _conversaoGatewayMock.Verify(g => g.EfetuarConversaoAsync(conversao, It.IsAny<CancellationToken>()), Times.Once);
    }
}