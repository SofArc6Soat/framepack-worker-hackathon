using Core.Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Conversao : Entity, IAggregateRoot
    {
        public Guid UsuarioId { get; private set; }
        public DateTime Data { get; private set; }
        public Status Status { get; private set; }
        public string NomeArquivo { get; private set; }

        public string UrlArquivoVideo { get; private set; }
        public string UrlArquivoCompactado { get; private set; } = string.Empty;

        public Conversao(Guid id, Guid usuarioId, DateTime data, Status status, string nomeArquivo, string urlArquivoVideo)
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
    }
}