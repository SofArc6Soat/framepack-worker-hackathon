using Domain.Entities;
using UseCases;

namespace Controllers
{
    public class PagamentoController(IPagamentoUseCase pagamentoUseCase) : IPagamentoController
    {
        public async Task<bool> EfetuarCheckoutAsync(Guid pedidoId, CancellationToken cancellationToken) =>
            await pagamentoUseCase.EfetuarCheckoutAsync(pedidoId, cancellationToken);

        public async Task<bool> NotificarPagamentoAsync(Guid pedidoId, CancellationToken cancellationToken) =>
            await pagamentoUseCase.NotificarPagamentoAsync(pedidoId, cancellationToken);

        public async Task<List<Pagamento>?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken) =>
            await pagamentoUseCase.ObterPagamentoPorPedidoAsync(pedidoId, cancellationToken);
    }
}
