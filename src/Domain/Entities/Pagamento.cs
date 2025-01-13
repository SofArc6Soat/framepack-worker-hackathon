using Core.Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Pagamento : Entity, IAggregateRoot
    {
        public Guid PedidoId { get; private set; }
        public StatusPagamento Status { get; private set; }
        public decimal Valor { get; private set; }
        public string? QrCodePix { get; private set; }
        public DateTime DataPagamento { get; private set; }

        public Pagamento(Guid pedidoId, decimal valor)
        {
            Id = Guid.NewGuid();
            PedidoId = pedidoId;
            Valor = valor;
            DataPagamento = DateTime.Now;
        }

        public Pagamento(Guid id, Guid pedidoId, StatusPagamento status, decimal valor, string? qrCodePix, DateTime dataCriacao)
        {
            Id = id;
            PedidoId = pedidoId;
            Status = status;
            Valor = valor;
            QrCodePix = qrCodePix;
            DataPagamento = dataCriacao;
        }

        public void AtribuirQrCodePix(string qrCodePix) =>
            QrCodePix = qrCodePix;

        public void AlterarStatusPagamentoParaPendente() =>
            Status = StatusPagamento.Pendente;

        public void AlterarStatusPagamentoParaPago() =>
            Status = StatusPagamento.Pago;
    }
}
