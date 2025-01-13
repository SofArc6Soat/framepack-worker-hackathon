using Domain.Entities;

namespace UseCases
{
    public interface IPagamentoUseCase
    {
        Task<List<Pagamento>?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<bool> EfetuarCheckoutAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<bool> NotificarPagamentoAsync(Guid pedidoId, CancellationToken cancellationToken);
    }
}