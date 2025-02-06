using Controllers;
using Moq;
using UseCases;

namespace Framepack.Worker.Tets.Adpters.Controllers;

public class ConversaoControllerTests
{
    private readonly Mock<IConversaoUseCase> _mockConversaoUseCase;
    private readonly ConversaoController _controller;

    public ConversaoControllerTests()
    {
        _mockConversaoUseCase = new Mock<IConversaoUseCase>();
        _controller = new ConversaoController(_mockConversaoUseCase.Object);
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_Success_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ProcessarConversaoSolicitadaAsync(id, cancellationToken);

        // Assert
        Assert.True(result);
        _mockConversaoUseCase.Verify(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_Failure_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ProcessarConversaoSolicitadaAsync(id, cancellationToken);

        // Assert
        Assert.False(result);
        _mockConversaoUseCase.Verify(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessarConversaoSolicitadaAsync_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken))
            .ThrowsAsync(new Exception("Erro ao processar conversão"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.ProcessarConversaoSolicitadaAsync(id, cancellationToken));
        _mockConversaoUseCase.Verify(x => x.ProcessarConversaoSolicitadaAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessarDownloadEfetuadoAsync_Success_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ProcessarDownloadEfetuadoAsync(id, cancellationToken);

        // Assert
        Assert.True(result);
        _mockConversaoUseCase.Verify(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessarDownloadEfetuadoAsync_Failure_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ProcessarDownloadEfetuadoAsync(id, cancellationToken);

        // Assert
        Assert.False(result);
        _mockConversaoUseCase.Verify(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessarDownloadEfetuadoAsync_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _mockConversaoUseCase.Setup(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken))
            .ThrowsAsync(new Exception("Erro ao processar download"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.ProcessarDownloadEfetuadoAsync(id, cancellationToken));
        _mockConversaoUseCase.Verify(x => x.ProcessarDownloadEfetuadoAsync(id, cancellationToken), Times.Once);
    }
}
