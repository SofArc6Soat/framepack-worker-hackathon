using Core.Domain.Base;
using Core.Domain.Notificacoes;
using Domain.Entities;
using Gateways;

namespace UseCases
{
    public class PagamentoUseCase(IPedidoGateway pedidoGateway, IPagamentoGateway pagamentoGateway, INotificador notificador) : BaseUseCase(notificador), IPagamentoUseCase
    {
        public async Task<bool> EfetuarCheckoutAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            var pedido = await pedidoGateway.ObterPedidoAsync(pedidoId, cancellationToken);

            if (pedido is null)
            {
                Notificar($"Pedido {pedidoId} não encontrado.");
                return false;
            }

            var pagamentoExistente = await pagamentoGateway.ObterPagamentoPorPedidoAsync(pedidoId, cancellationToken);

            if (pagamentoExistente is not null)
            {
                Notificar("Pagamento já existente para o pedido, aguarde a confirmação do seu Pix.");
                return false;
            }

            if (await pedidoGateway.AtualizarPedidoAsync(pedido, cancellationToken))
            {
                var pagamento = new Pagamento(pedidoId, pedido.ValorTotal);

                var qrCodePix = pagamentoGateway.GerarQrCodePixGatewayPagamento(pagamento);

                pagamento.AtribuirQrCodePix(qrCodePix);
                pagamento.AlterarStatusPagamentoParaPendente();

                return await pagamentoGateway.CadastrarPagamentoAsync(pagamento, cancellationToken);
            }

            return false;
        }

        public async Task<bool> NotificarPagamentoAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            var pagamento = await pagamentoGateway.ObterPagamentoPorPedidoAsync(pedidoId, cancellationToken);

            if (pagamento is null)
            {
                Notificar($"Pagamento inexistente para o {pedidoId}, verifique se o pedido foi efetuado corretamente");
                return false;
            }

            var pedido = await pedidoGateway.ObterPedidoAsync(pedidoId, cancellationToken);

            if (pedido is null)
            {
                Notificar($"Pedido {pedidoId} não encontrado.");
                return false;
            }

            pagamento.AlterarStatusPagamentoParaPago();

            return await pedidoGateway.AtualizarPedidoAsync(pedido, cancellationToken) && await pagamentoGateway.NotificarPagamentoAsync(pagamento, cancellationToken);
        }

        public async Task<List<Pagamento>?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken) =>
            await pagamentoGateway.ObterPagamentosPorPedidoAsync(pedidoId, cancellationToken);
    }
}
