using Core.Infra.S3;
using Gateways.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Framepack.Worker.Tets.Adpters.Gateways.Handlers;

public class ArquivoHandlerTests
{
    private readonly Mock<ILogger<ArquivoHandler>> _loggerMock;
    private readonly Mock<IS3Service> _s3ServiceMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly ArquivoHandler _arquivoHandler;

    public ArquivoHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ArquivoHandler>>();
        _s3ServiceMock = new Mock<IS3Service>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _arquivoHandler = new ArquivoHandler(_loggerMock.Object, _httpClientFactoryMock.Object);
    }

    //[Fact]
    //public async Task DownloadVideoAsync_Success()
    //{
    //    // Arrange
    //    var id = Guid.NewGuid();
    //    var url = "http://example.com/video.mp4";
    //    var videoPath = Path.Combine(Path.GetTempPath(), "video-processing", $"{id}.mp4");

    //    _httpMessageHandlerMock
    //        .Protected()
    //        .Setup<Task<HttpResponseMessage>>(
    //            "SendAsync",
    //            ItExpr.IsAny<HttpRequestMessage>(),
    //            ItExpr.IsAny<CancellationToken>()
    //        )
    //        .ReturnsAsync(new HttpResponseMessage
    //        {
    //            StatusCode = HttpStatusCode.OK,
    //            Content = new ByteArrayContent(new byte[] { 1, 2, 3 })
    //        });

    //    // Act
    //    var result = await _arquivoHandler.DownloadVideoAsync(id, url);

    //    // Assert
    //    Assert.Equal(videoPath, result);
    //    Assert.True(File.Exists(videoPath));
    //    File.Delete(videoPath);
    //}

    //[Fact]
    //public async Task DownloadVideoAsync_Failure()
    //{
    //    // Arrange
    //    var id = Guid.NewGuid();
    //    var url = "http://example.com/video.mp4";

    //    _httpMessageHandlerMock
    //        .Protected()
    //        .Setup<Task<HttpResponseMessage>>(
    //            "SendAsync",
    //            ItExpr.IsAny<HttpRequestMessage>(),
    //            ItExpr.IsAny<CancellationToken>()
    //        )
    //        .ReturnsAsync(new HttpResponseMessage
    //        {
    //            StatusCode = HttpStatusCode.NotFound
    //        });

    //    // Act & Assert
    //    await Assert.ThrowsAsync<HttpRequestException>(() => _arquivoHandler.DownloadVideoAsync(id, url));
    //}

    [Fact]
    public async Task CompactarUploadFramesAsync_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var framesPath = Path.Combine(Path.GetTempPath(), "frames");
        var videoPath = Path.Combine(Path.GetTempPath(), "video-processing", $"{id}.mp4");
        var zipPath = Path.Combine(Path.GetTempPath(), "video-output", $"{id}.zip");

        Directory.CreateDirectory(framesPath);
        File.WriteAllText(Path.Combine(framesPath, "frame1.txt"), "frame1");
        File.WriteAllText(videoPath, "video");

        _s3ServiceMock.Setup(s => s.UploadArquivoAsync(zipPath)).ReturnsAsync("http://example.com/zip");

        // Act
        var result = await _arquivoHandler.CompactarUploadFramesAsync(id, framesPath, videoPath, _s3ServiceMock.Object);

        // Assert
        Assert.Equal("http://example.com/zip", result);
        Assert.False(Directory.Exists(framesPath));
        Assert.False(File.Exists(videoPath));
        Assert.False(File.Exists(zipPath));
    }

    [Fact]
    public async Task LimpezaAsync_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var url = "http://example.com/video.mp4";
        var videoPath = Path.Combine(Path.GetTempPath(), "video-processing", $"{id}.mp4");
        var framesPath = Path.Combine(Path.GetTempPath(), "video-processing", $"{id}");

        Directory.CreateDirectory(framesPath);
        File.WriteAllText(videoPath, "video");

        _s3ServiceMock.Setup(s => s.DeletarArquivoAsync(url)).Returns(Task.CompletedTask);

        // Act
        await _arquivoHandler.LimpezaAsync(id, url, _s3ServiceMock.Object);

        // Assert
        Assert.False(Directory.Exists(framesPath));
        Assert.False(File.Exists(videoPath));
    }
}
