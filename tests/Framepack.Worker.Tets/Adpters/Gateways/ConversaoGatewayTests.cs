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
        private readonly Mock<IEmailService> _emailServiceMock;

        public ConversaoGatewayTests()
        {
            _repositoryMock = new Mock<IDynamoDBContext>();
            _s3ServiceMock = new Mock<IS3Service>();
            _videoHandlerMock = new Mock<IVideoHandler>();
            _arquivoHandlerMock = new Mock<IArquivoHandler>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        private ConversaoGateway CreateGateway() =>
            new(_repositoryMock.Object,
                _s3ServiceMock.Object,
                _videoHandlerMock.Object,
                _arquivoHandlerMock.Object,
                _emailServiceMock.Object);

        private static Mock<Conversao> CreateMockConversao(Guid? id = null)
        {
            var convId = id ?? Guid.NewGuid();
            var usuarioId = "usuario";
            var data = DateTime.UtcNow;
            var conversao = new Conversao(convId, usuarioId, data, Status.Processando, "video.mp4", "http://video-url");
            conversao.SetEmailUsuario("teste@teste.com");

            var mockConversao = new Mock<Conversao>(convId, usuarioId, data, conversao.Status, conversao.NomeArquivo, conversao.UrlArquivoVideo)
            {
                CallBase = true
            };

            mockConversao.Object.SetEmailUsuario("teste@teste.com");
            return mockConversao;
        }

        private FakeConversao CreateFakeConversao()
        {
            var id = Guid.NewGuid();
            var usuarioId = "usuario";
            var data = DateTime.UtcNow;

            var fake = new FakeConversao(id, usuarioId, data, Status.Processando, "video.mp4", "http://video-url");
            fake.SetEmailUsuario("teste@teste.com");
            return fake;
        }

        [Fact]
        public async Task ObterConversaoAsync_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var gateway = CreateGateway();
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.LoadAsync<ConversaoDb>(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ConversaoDb?)null);

            // Act
            var result = await gateway.ObterConversaoAsync(id, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterConversaoAsync_ReturnsMappedConversao_WhenRepositoryReturnsConversaoDb()
        {
            // Arrange
            var gateway = CreateGateway();
            var id = Guid.NewGuid();
            var usuarioId = "usuario";
            var data = DateTime.UtcNow;
            var conversaoDb = new ConversaoDb
            {
                Id = id,
                UsuarioId = usuarioId,
                Data = data,
                Status = "Processando",
                NomeArquivo = "video.mp4",
                UrlArquivoVideo = "http://video-url",
                UrlArquivoCompactado = "http://compactado-url",
                EmailUsuario = "teste@teste.com"
            };
            _repositoryMock
                .Setup(r => r.LoadAsync<ConversaoDb>(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversaoDb);

            // Act
            var result = await gateway.ObterConversaoAsync(id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conversaoDb.Id, result.Id);
            Assert.Equal(conversaoDb.UsuarioId, result.UsuarioId);
            Assert.Equal(conversaoDb.Data, result.Data);
            Assert.Equal(Enum.Parse<Status>(conversaoDb.Status), result.Status);
            Assert.Equal(conversaoDb.NomeArquivo, result.NomeArquivo);
            Assert.Equal(conversaoDb.UrlArquivoVideo, result.UrlArquivoVideo);
            Assert.Equal(conversaoDb.UrlArquivoCompactado, result.UrlArquivoCompactado);
            Assert.Equal(conversaoDb.EmailUsuario, result.EmailUsuario);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_ReturnsFalse_WhenAlterarStatusParaCompactandoFails()
        {
            // Arrange
            var gateway = CreateGateway();
            var cancellationToken = CancellationToken.None;
            var fakeConversao = CreateFakeConversao();

            fakeConversao.RetornoAlterarStatusProcessando = true;
            fakeConversao.RetornoAlterarStatusCompactando = false;

            var preSignedUrl = "http://pre-signed-url";
            var videoPath = "videoPath";
            var framesPath = "framesPath";

            _s3ServiceMock
                .Setup(s => s.GerarPreSignedUrl(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(preSignedUrl);
            _arquivoHandlerMock
                .Setup(a => a.DownloadVideoAsync(fakeConversao.Id, preSignedUrl))
                .ReturnsAsync(videoPath);
            _videoHandlerMock
                .Setup(v => v.ExtrairFramesAsync(fakeConversao.Id, videoPath))
                .ReturnsAsync(framesPath);
            _arquivoHandlerMock
                .Setup(a => a.LimpezaAsync(fakeConversao.Id, fakeConversao.UrlArquivoVideo, _s3ServiceMock.Object))
                .Returns(Task.CompletedTask);
            _repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken))
                .Returns(Task.CompletedTask);
            _emailServiceMock
                .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await gateway.EfetuarConversaoAsync(fakeConversao, cancellationToken);

            // Assert
            Assert.False(result);
            _emailServiceMock.Verify(es => es.SendEmailAsync(
                It.IsAny<string>(),
                "Conversão realizada com erro",
                It.Is<string>(msg => msg.Contains(fakeConversao.Id.ToString()))),
                Times.Once);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_ReturnsFalse_WhenCompactarUploadFramesAsyncReturnsEmpty()
        {
            // Arrange
            var gateway = CreateGateway();
            var cancellationToken = CancellationToken.None;
            var fakeConversao = CreateFakeConversao();

            fakeConversao.RetornoAlterarStatusProcessando = true;
            fakeConversao.RetornoAlterarStatusCompactando = true;

            var preSignedUrl = "http://pre-signed-url";
            var videoPath = "videoPath";
            var framesPath = "framesPath";
            var urlArquivoCompactado = string.Empty;

            _s3ServiceMock
                .Setup(s => s.GerarPreSignedUrl(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(preSignedUrl);
            _arquivoHandlerMock
                .Setup(a => a.DownloadVideoAsync(fakeConversao.Id, preSignedUrl))
                .ReturnsAsync(videoPath);
            _videoHandlerMock
                .Setup(v => v.ExtrairFramesAsync(fakeConversao.Id, videoPath))
                .ReturnsAsync(framesPath);
            _arquivoHandlerMock
                .Setup(a => a.CompactarUploadFramesAsync(fakeConversao.Id, framesPath, videoPath, _s3ServiceMock.Object))
                .ReturnsAsync(urlArquivoCompactado);
            _arquivoHandlerMock
                .Setup(a => a.LimpezaAsync(fakeConversao.Id, fakeConversao.UrlArquivoVideo, _s3ServiceMock.Object))
                .Returns(Task.CompletedTask);
            _repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken))
                .Returns(Task.CompletedTask);
            _emailServiceMock
                .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await gateway.EfetuarConversaoAsync(fakeConversao, cancellationToken);

            // Assert
            Assert.False(result);
            _emailServiceMock.Verify(es => es.SendEmailAsync(
                It.IsAny<string>(),
                "Conversão realizada com erro",
                It.Is<string>(msg => msg.Contains(fakeConversao.Id.ToString()))),
                Times.Once);
        }

        [Fact]
        public async Task EfetuarConversaoAsync_ReturnsFalse_WhenAlterarStatusParaProcessandoFails()
        {
            // Arrange
            var gateway = CreateGateway();
            var cancellationToken = CancellationToken.None;
            var fakeConversao = CreateFakeConversao();

            fakeConversao.RetornoAlterarStatusProcessando = false;

            _repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken))
                .Returns(Task.CompletedTask);
            _emailServiceMock
                .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await gateway.EfetuarConversaoAsync(fakeConversao, cancellationToken);

            // Assert
            Assert.False(result);
            _emailServiceMock.Verify(es => es.SendEmailAsync(
                It.IsAny<string>(),
                "Conversão realizada com erro",
                It.Is<string>(msg => msg.Contains(fakeConversao.Id.ToString()))),
                Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DownloadEfetuadoAsync_ReturnsTrue_WhenExclusaoArquivoAposDownloadSucceeds()
        {
            // Arrange
            var gateway = CreateGateway();
            var cancellationToken = CancellationToken.None;
            var conversao = CreateMockConversao().Object;

            _arquivoHandlerMock
                .Setup(a => a.ExcluiArquivoAposDownloadAsync(conversao.Id, conversao.UrlArquivoCompactado, _s3ServiceMock.Object))
                .ReturnsAsync(true);
            _repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await gateway.DownloadEfetuadoAsync(conversao, cancellationToken);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DownloadEfetuadoAsync_ReturnsFalse_WhenExclusaoArquivoAposDownloadFails()
        {
            // Arrange
            var gateway = CreateGateway();
            var cancellationToken = CancellationToken.None;
            var conversao = CreateMockConversao().Object;

            _arquivoHandlerMock
                .Setup(a => a.ExcluiArquivoAposDownloadAsync(conversao.Id, conversao.UrlArquivoCompactado, _s3ServiceMock.Object))
                .ReturnsAsync(false);

            // Act
            var result = await gateway.DownloadEfetuadoAsync(conversao, cancellationToken);

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<ConversaoDb>(), cancellationToken), Times.Never);
        }
    }
}