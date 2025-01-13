using Domain.Entities;

namespace Gateways
{
    public interface IPagamentoGateway
    {
        Task<Pagamento?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<List<Pagamento>?> ObterPagamentosPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<bool> CadastrarPagamentoAsync(Pagamento pagamento, CancellationToken cancellationToken);
        Task<bool> NotificarPagamentoAsync(Pagamento pagamento, CancellationToken cancellationToken);
        string GerarQrCodePixGatewayPagamento(Pagamento pagamento);
    }
}
