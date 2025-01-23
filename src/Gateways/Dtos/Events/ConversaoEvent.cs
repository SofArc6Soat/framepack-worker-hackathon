using Core.Domain.Entities;

namespace Gateways.Dtos.Events
{
    public record ConversaoSolicitadaEvent : Event
    {
        public Guid UsuarioId { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;
        public string UrlArquivoVideo { get; set; } = string.Empty;
    }
}
