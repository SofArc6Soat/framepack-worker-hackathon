using Core.Domain.Entities;

namespace Gateways.Dtos.Events;

public record ConversaoSolicitadaEvent : Event
{
    private DateTime _data;

    public string UsuarioId { get; set; } = string.Empty;
    public DateTime Data
    {
        get => _data;
        set
        {
            if (value == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(Data), "Data não pode ser DateTime.MinValue");
            }
            _data = value;
        }
    }
    public string Status { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string UrlArquivoVideo { get; set; } = string.Empty;
}