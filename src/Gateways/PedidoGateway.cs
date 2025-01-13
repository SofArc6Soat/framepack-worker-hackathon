using Amazon.DynamoDBv2.DataModel;
using Domain.Entities;
using Infra.Dto;

namespace Gateways
{
    public class PedidoGateway(IDynamoDBContext repository) : IPedidoGateway
    {
        public async Task<Pedido?> ObterPedidoAsync(Guid id, CancellationToken cancellationToken)
        {
            var pedidoDto = await repository.LoadAsync<PedidoDb>(id, cancellationToken);

            return pedidoDto is null ? null : new Pedido(pedidoDto.Id, pedidoDto.NumeroPedido, pedidoDto.ValorTotal, pedidoDto.DataPedido);
        }

        public async Task<bool> AtualizarPedidoAsync(Pedido pedido, CancellationToken cancellationToken)
        {
            var pedidoDto = new PedidoDb
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                ValorTotal = pedido.ValorTotal,
                DataPedido = pedido.DataPedido
            };

            await repository.SaveAsync(pedidoDto, cancellationToken);

            return true;
        }
    }
}
