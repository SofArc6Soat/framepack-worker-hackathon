using Core.Domain.Entities;

namespace Gateways.Dtos.Events
{
    public record PedidoPendentePagamentoEvent : Event
    {
        public Guid PedidoId { get; set; }
        public string Status { get; set; } = "PendentePagamento";
    }
}
