using Domain.Entities;
using Domain.ValueObjects;

namespace Framepack.Worker.Tets.Core.Entities;

public class ConversaoTests
{
    [Fact]
    public void Deve_Criar_Conversao_Com_Sucesso()
    {
        // Arrange
        var id = Guid.NewGuid();
        var usuarioId = "usuario123";
        var data = DateTime.Now;
        var status = Status.AguardandoConversao;
        var nomeArquivo = "video.mp4";
        var urlArquivoVideo = "http://example.com/video.mp4";

        // Act
        var conversao = new Conversao(id, usuarioId, data, status, nomeArquivo, urlArquivoVideo);

        // Assert
        Assert.Equal(id, conversao.Id);
        Assert.Equal(usuarioId, conversao.UsuarioId);
        Assert.Equal(data, conversao.Data);
        Assert.Equal(status, conversao.Status);
        Assert.Equal(nomeArquivo, conversao.NomeArquivo);
        Assert.Equal(urlArquivoVideo, conversao.UrlArquivoVideo);
        Assert.Equal(string.Empty, conversao.UrlArquivoCompactado);
    }

    [Fact]
    public void Deve_Alterar_Status_Para_Processando_Com_Sucesso()
    {
        // Arrange
        var conversao = CriarConversao(Status.AguardandoConversao);

        // Act
        var resultado = conversao.AlterarStatusParaProcessando(conversao.Status);

        // Assert
        Assert.True(resultado);
        Assert.Equal(Status.Processando, conversao.Status);
    }

    [Fact]
    public void Nao_Deve_Alterar_Status_Para_Processando_Se_Status_Nao_For_AguardandoConversao()
    {
        // Arrange
        var conversao = CriarConversao(Status.Processando);

        // Act
        var resultado = conversao.AlterarStatusParaProcessando(conversao.Status);

        // Assert
        Assert.False(resultado);
        Assert.Equal(Status.Processando, conversao.Status);
    }

    [Fact]
    public void Deve_Alterar_Status_Para_Compactando_Com_Sucesso()
    {
        // Arrange
        var conversao = CriarConversao(Status.Processando);

        // Act
        var resultado = conversao.AlterarStatusParaCompactando(conversao.Status);

        // Assert
        Assert.True(resultado);
        Assert.Equal(Status.Compactando, conversao.Status);
    }

    [Fact]
    public void Nao_Deve_Alterar_Status_Para_Compactando_Se_Status_Nao_For_Processando()
    {
        // Arrange
        var conversao = CriarConversao(Status.AguardandoConversao);

        // Act
        var resultado = conversao.AlterarStatusParaCompactando(conversao.Status);

        // Assert
        Assert.False(resultado);
        Assert.Equal(Status.AguardandoConversao, conversao.Status);
    }

    [Fact]
    public void Deve_Alterar_Status_Para_Concluido_Com_Sucesso()
    {
        // Arrange
        var conversao = CriarConversao(Status.Compactando);

        // Act
        var resultado = conversao.AlterarStatusParaConcluido(conversao.Status);

        // Assert
        Assert.True(resultado);
        Assert.Equal(Status.Concluido, conversao.Status);
    }

    [Fact]
    public void Nao_Deve_Alterar_Status_Para_Concluido_Se_Status_Nao_For_Compactando()
    {
        // Arrange
        var conversao = CriarConversao(Status.Processando);

        // Act
        var resultado = conversao.AlterarStatusParaConcluido(conversao.Status);

        // Assert
        Assert.False(resultado);
        Assert.Equal(Status.Processando, conversao.Status);
    }

    [Fact]
    public void Deve_Alterar_Status_Para_Erro()
    {
        // Arrange
        var conversao = CriarConversao(Status.Processando);

        // Act
        conversao.AlterarStatusParaErro();

        // Assert
        Assert.Equal(Status.Erro, conversao.Status);
    }

    [Fact]
    public void Deve_Definir_UrlArquivoCompactado()
    {
        // Arrange
        var conversao = CriarConversao(Status.Processando);
        var urlArquivoCompactado = "http://example.com/video.zip";

        // Act
        conversao.SetUrlArquivoCompactado(urlArquivoCompactado);

        // Assert
        Assert.Equal(urlArquivoCompactado, conversao.UrlArquivoCompactado);
    }

    private Conversao CriarConversao(Status status) => new(
            Guid.NewGuid(),
            "usuario123",
            DateTime.Now,
            status,
            "video.mp4",
            "http://example.com/video.mp4"
        );
}