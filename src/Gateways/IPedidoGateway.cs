using Domain.Entities;

namespace Gateways
{
    public interface IPedidoGateway
    {
        Task<Pedido?> ObterPedidoAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> AtualizarPedidoAsync(Pedido pedido, CancellationToken cancellationToken);
    }
}
