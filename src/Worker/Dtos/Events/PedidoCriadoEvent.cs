using Core.Domain.Entities;

namespace Worker.Dtos.Events
{
    public record PedidoCriadoEvent : Event
    {
        public int NumeroPedido { get; set; }
        public Guid? ClienteId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public DateTime DataPedido { get; set; }
    }
}
