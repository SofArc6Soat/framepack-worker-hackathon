using Core.Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Conversao : Entity, IAggregateRoot
    {
        public string UsuarioId { get; private set; }
        public DateTime Data { get; private set; }
        public Status Status { get; private set; }
        public string NomeArquivo { get; private set; }

        public string UrlArquivoVideo { get; private set; }
        public string UrlArquivoCompactado { get; private set; } = string.Empty;

        public Conversao(Guid id, string usuarioId, DateTime data, Status status, string nomeArquivo, string urlArquivoVideo)
        {
            Id = id;
            UsuarioId = usuarioId;
            Data = data;
            Status = status;
            NomeArquivo = nomeArquivo;
            UrlArquivoVideo = urlArquivoVideo;
        }

        public void SetUrlArquivoCompactado(string urlArquivoCompactado) =>
            UrlArquivoCompactado = urlArquivoCompactado;

        public bool AlterarStatusParaProcessando(Status statusAtual)
        {
            if (statusAtual == Status.AguardandoConversao)
            {
                Status = Status.Processando;
                return true;
            }

            return false;
        }

        public bool AlterarStatusParaCompactando(Status statusAtual)
        {
            if (statusAtual == Status.Processando)
            {
                Status = Status.Compactando;
                return true;
            }

            return false;
        }

        public bool AlterarStatusParaConcluido(Status statusAtual)
        {
            if (statusAtual == Status.Compactando)
            {
                Status = Status.Concluido;
                return true;
            }

            return false;
        }

        public void AlterarStatusParaErro() =>
            Status = Status.Erro;
    }
}