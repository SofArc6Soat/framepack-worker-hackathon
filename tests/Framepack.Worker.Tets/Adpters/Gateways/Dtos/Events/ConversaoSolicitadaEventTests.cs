using Gateways.Dtos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framepack.Worker.Tets.Adpters.Gateways.Dtos.Events;

public class ConversaoSolicitadaEventTests
{
    [Fact]
    public void ConversaoSolicitadaEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var evento = new ConversaoSolicitadaEvent();

        // Assert
        Assert.Equal(string.Empty, evento.UsuarioId);
        Assert.Equal(default(DateTime), evento.Data);
        Assert.Equal(string.Empty, evento.Status);
        Assert.Equal(string.Empty, evento.NomeArquivo);
        Assert.Equal(string.Empty, evento.UrlArquivoVideo);
    }

    [Fact]
    public void ConversaoSolicitadaEvent_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var usuarioId = "user123";
        var data = DateTime.Now;
        var status = "Pending";
        var nomeArquivo = "video.mp4";
        var urlArquivoVideo = "http://example.com/video.mp4";

        // Act
        var evento = new ConversaoSolicitadaEvent
        {
            UsuarioId = usuarioId,
            Data = data,
            Status = status,
            NomeArquivo = nomeArquivo,
            UrlArquivoVideo = urlArquivoVideo
        };

        // Assert
        Assert.Equal(usuarioId, evento.UsuarioId);
        Assert.Equal(data, evento.Data);
        Assert.Equal(status, evento.Status);
        Assert.Equal(nomeArquivo, evento.NomeArquivo);
        Assert.Equal(urlArquivoVideo, evento.UrlArquivoVideo);
    }

    [Fact]
    public void ConversaoSolicitadaEvent_ShouldThrowExceptionForInvalidData()
    {
        // Arrange
        var evento = new ConversaoSolicitadaEvent();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => evento.Data = DateTime.MinValue);
    }
}
