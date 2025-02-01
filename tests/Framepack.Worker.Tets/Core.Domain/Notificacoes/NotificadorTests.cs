using Core.Domain.Notificacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framepack.Worker.Tets.Core.Domain.Notificacoes;

public class NotificadorTests
{
    [Fact]
    public void Notificador_DeveIniciarSemNotificacoes()
    {
        // Arrange
        var notificador = new Notificador();

        // Act
        var temNotificacao = notificador.TemNotificacao();
        var notificacoes = notificador.ObterNotificacoes();

        // Assert
        Assert.False(temNotificacao);
        Assert.Empty(notificacoes);
    }

    [Fact]
    public void Notificador_DeveAdicionarNotificacao()
    {
        // Arrange
        var notificador = new Notificador();
        var notificacao = new Notificacao("Teste de notificação");

        // Act
        notificador.Handle(notificacao);
        var temNotificacao = notificador.TemNotificacao();
        var notificacoes = notificador.ObterNotificacoes();

        // Assert
        Assert.True(temNotificacao);
        Assert.Single(notificacoes);
        Assert.Equal("Teste de notificação", notificacoes[0].Mensagem);
    }

    [Fact]
    public void Notificador_DeveAdicionarMultiplasNotificacoes()
    {
        // Arrange
        var notificador = new Notificador();
        var notificacao1 = new Notificacao("Teste de notificação 1");
        var notificacao2 = new Notificacao("Teste de notificação 2");

        // Act
        notificador.Handle(notificacao1);
        notificador.Handle(notificacao2);
        var temNotificacao = notificador.TemNotificacao();
        var notificacoes = notificador.ObterNotificacoes();

        // Assert
        Assert.True(temNotificacao);
        Assert.Equal(2, notificacoes.Count);
        Assert.Equal("Teste de notificação 1", notificacoes[0].Mensagem);
        Assert.Equal("Teste de notificação 2", notificacoes[1].Mensagem);
    }
}
