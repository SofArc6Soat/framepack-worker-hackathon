using Amazon.DynamoDBv2.DataModel;
using Core.Infra.EmailSender;
using Core.Infra.S3;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways;
using Gateways.Handlers;
using Infra.Dto;
using Moq;

namespace Framepack.Worker.Tets.Adpters.Gateways
{
    public class ConversaoGatewayTests
    {
        private readonly Mock<IDynamoDBContext> _repositoryMock;
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly Mock<IVideoHandler> _videoHandlerMock;
        private readonly Mock<IArquivoHandler> _arquivoHandlerMock;
        private readonly Mock<IEmailService> _emailSenderMock;
        private readonly ConversaoGateway _conversaoGateway;

        public ConversaoGatewayTests()
        {
            _repositoryMock = new Mock<IDynamoDBContext>();
            _s3ServiceMock = new Mock<IS3Service>();
            _videoHandlerMock = new Mock<IVideoHandler>();
            _arquivoHandlerMock = new Mock<IArquivoHandler>();
            _emailSenderMock = new Mock<IEmailService>();
            _conversaoGateway = new ConversaoGateway(
                _repositoryMock.Object,
                _s3ServiceMock.Object,
                _videoHandlerMock.Object,
                _arquivoHandlerMock.Object,
                _emailSenderMock.Object
            );
        }

        [Fact]
        public async Task ObterConversaoAsync_DeveRetornarConversao_QuandoEncontrada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var conversaoDb = new ConversaoDb
            {
                Id = id,
                UsuarioId = "user1",
                Status = Status.AguardandoConversao.ToString(),
                Data = DateTime.UtcNow,
                NomeArquivo = "video.mp4",
                UrlArquivoVideo = "http://example.com/video.mp4"
            };
            _repositoryMock.Setup(r => r.LoadAsync<ConversaoDb>(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversaoDb);

            // Act
            var result = await _conversaoGateway.ObterConversaoAsync(id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task ObterConversaoAsync_DeveRetornarNull_QuandoNaoEncontrada()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.LoadAsync<ConversaoDb>(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ConversaoDb)null);

            // Act
            var result = await _conversaoGateway.ObterConversaoAsync(id, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_DeveRetornarTrue_QuandoConversaoConcluidaComSucesso()
        {
            // Arrange
            var conversao = new Conversao(Guid.NewGuid(), "user1", DateTime.UtcNow, Status.AguardandoConversao, "video.mp4", "http://example.com/video.mp4");
            _arquivoHandlerMock.Setup(a => a.DownloadVideoAsync(conversao.Id, It.IsAny<string>()))
                .ReturnsAsync("localVideoPath");
            _videoHandlerMock.Setup(v => v.ExtrairFramesAsync(conversao.Id, "localVideoPath"))
                .ReturnsAsync("framesPath");
            _arquivoHandlerMock.Setup(a => a.CompactarUploadFramesAsync(conversao.Id, "framesPath", "localVideoPath", _s3ServiceMock.Object))
                .ReturnsAsync("http://example.com/compactado.mp4");

            // Act
            var result = await _conversaoGateway.EfetuarConversaoAsync(conversao, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            _arquivoHandlerMock.Verify(a => a.LimpezaAsync(conversao.Id, conversao.UrlArquivoVideo, _s3ServiceMock.Object), Times.Once);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_DeveRetornarFalse_QuandoFalhaAoAlterarStatusParaProcessando()
        {
            // Arrange
            var conversao = new Conversao(Guid.NewGuid(), "user1", DateTime.UtcNow, Status.Concluido, "video.mp4", "http://example.com/video.mp4");

            // Act
            var result = await _conversaoGateway.EfetuarConversaoAsync(conversao, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_DeveLancarExcecao_QuandoOcorreErroDuranteProcessamento()
        {
            // Arrange
            var conversao = new Conversao(Guid.NewGuid(), "user1", DateTime.UtcNow, Status.AguardandoConversao, "video.mp4", "http://example.com/video.mp4");
            _arquivoHandlerMock.Setup(a => a.DownloadVideoAsync(conversao.Id, It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro ao baixar vídeo"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _conversaoGateway.EfetuarConversaoAsync(conversao, CancellationToken.None));
            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _arquivoHandlerMock.Verify(a => a.LimpezaAsync(conversao.Id, conversao.UrlArquivoVideo, _s3ServiceMock.Object), Times.Once);
        }
    }
}