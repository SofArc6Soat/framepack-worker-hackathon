using System;
using Gateways.Dtos.Events;
using Xunit;

namespace Framepack.Worker.Tets.Adpters.Gateways.Dtos.Events;

public class ConversaoSolicitadaEventTests
{
    [Fact]
    public void Should_Create_ConversaoSolicitadaEvent_With_Valid_Data()
    {
        // Arrange
        var usuarioId = "user123";
        var emailUsuario = "user@example.com";
        var data = DateTime.Now;
        var status = "Pending";
        var nomeArquivo = "video.mp4";
        var urlArquivoVideo = "http://example.com/video.mp4";

        // Act
        var evento = new ConversaoSolicitadaEvent
        {
            UsuarioId = usuarioId,
            EmailUsuario = emailUsuario,
            Data = data,
            Status = status,
            NomeArquivo = nomeArquivo,
            UrlArquivoVideo = urlArquivoVideo
        };

        // Assert
        Assert.Equal(usuarioId, evento.UsuarioId);
        Assert.Equal(emailUsuario, evento.EmailUsuario);
        Assert.Equal(data, evento.Data);
        Assert.Equal(status, evento.Status);
        Assert.Equal(nomeArquivo, evento.NomeArquivo);
        Assert.Equal(urlArquivoVideo, evento.UrlArquivoVideo);
    }

    [Fact]
    public void Should_Throw_ArgumentOutOfRangeException_When_Data_Is_MinValue()
    {
        // Arrange
        var evento = new ConversaoSolicitadaEvent();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => evento.Data = DateTime.MinValue);
        Assert.Equal("Data não pode ser DateTime.MinValue (Parameter 'Data')", exception.Message);
    }

    [Fact]
    public void Should_Set_Data_When_Valid()
    {
        // Arrange
        var evento = new ConversaoSolicitadaEvent();
        var validDate = DateTime.Now;

        // Act
        evento.Data = validDate;

        // Assert
        Assert.Equal(validDate, evento.Data);
    }
}

public class DownloadEfetuadoEventTests
{
    [Fact]
    public void Should_Create_DownloadEfetuadoEvent_With_Valid_Data()
    {
        // Arrange
        var urlArquivoVideo = "http://example.com/video.mp4";

        // Act
        var evento = new DownloadEfetuadoEvent
        {
            UrlArquivoVideo = urlArquivoVideo
        };

        // Assert
        Assert.Equal(urlArquivoVideo, evento.UrlArquivoVideo);
    }

    [Fact]
    public void Should_Set_UrlArquivoVideo_When_Valid()
    {
        // Arrange
        var evento = new DownloadEfetuadoEvent();
        var urlArquivoVideo = "http://example.com/video.mp4";

        // Act
        evento.UrlArquivoVideo = urlArquivoVideo;

        // Assert
        Assert.Equal(urlArquivoVideo, evento.UrlArquivoVideo);
    }
}
