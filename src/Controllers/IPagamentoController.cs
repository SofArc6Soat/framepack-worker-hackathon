using Domain.Entities;

namespace Controllers
{
    public interface IPagamentoController
    {
        Task<List<Pagamento>?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<bool> EfetuarCheckoutAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<bool> NotificarPagamentoAsync(Guid pedidoId, CancellationToken cancellationToken);
    }
}
