using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Infra.MessageBroker;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways.Dtos.Events;
using Infra.Dto;
using System.Security.Cryptography;

namespace Gateways
{
    public class PagamentoGateway(ISqsService<PedidoPagoEvent> sqsPedidoPago, ISqsService<PedidoPendentePagamentoEvent> sqsPedidoPendentePagamento, IDynamoDBContext repository) : IPagamentoGateway
    {
        public async Task<bool> CadastrarPagamentoAsync(Pagamento pagamento, CancellationToken cancellationToken)
        {
            var pagementoDto = new PagamentoDb
            {
                Id = pagamento.Id,
                PedidoId = pagamento.PedidoId,
                Status = pagamento.Status.ToString(),
                QrCodePix = pagamento.QrCodePix,
                Valor = pagamento.Valor,
                DataPagamento = pagamento.DataPagamento
            };

            await repository.SaveAsync(pagementoDto, cancellationToken);

            return await sqsPedidoPendentePagamento.SendMessageAsync(GerarPedidoPendentePagamentoEvent(pagementoDto));
        }

        public async Task<bool> NotificarPagamentoAsync(Pagamento pagamento, CancellationToken cancellationToken)
        {
            var pagementoDto = new PagamentoDb
            {
                Id = pagamento.Id,
                PedidoId = pagamento.PedidoId,
                Status = pagamento.Status.ToString(),
                QrCodePix = pagamento.QrCodePix,
                Valor = pagamento.Valor,
                DataPagamento = pagamento.DataPagamento
            };

            await repository.SaveAsync(pagementoDto, cancellationToken);

            return await sqsPedidoPago.SendMessageAsync(GerarPedidoPagoEvent(pagementoDto));
        }

        public string GerarQrCodePixGatewayPagamento(Pagamento pagamento)
        {
            // Integração com gateway de pagamento e geração QR Code do PIX

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringLength = 100;

            var result = new char[stringLength];
            var charsLength = chars.Length;

            var randomBytes = new byte[stringLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (var i = 0; i < stringLength; i++)
            {
                result[i] = chars[randomBytes[i] % charsLength];
            }

            return new string(result);
        }

        public async Task<Pagamento?> ObterPagamentoPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            var conditions = new List<ScanCondition>
            {
                new("PedidoId", ScanOperator.Equal, pedidoId)
            };

            var pagamentos = await repository.ScanAsync<PagamentoDb>(conditions).GetRemainingAsync(cancellationToken);

            var pagamentoDto = pagamentos.FirstOrDefault();

            if (pagamentoDto is null)
            {
                return null;
            }

            _ = Enum.TryParse(pagamentoDto.Status, out StatusPagamento statusPagamento);

            return new Pagamento(pagamentoDto.Id, pagamentoDto.PedidoId, statusPagamento, pagamentoDto.Valor, pagamentoDto.QrCodePix, pagamentoDto.DataPagamento);
        }

        public async Task<List<Pagamento>?> ObterPagamentosPorPedidoAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            var conditions = new List<ScanCondition>
            {
                new("PedidoId", ScanOperator.Equal, pedidoId)
            };

            var pagamentosDb = await repository.ScanAsync<PagamentoDb>(conditions).GetRemainingAsync(cancellationToken);

            return pagamentosDb.Select(item => ToPagamento(item)).ToList();
        }


        private static PedidoPagoEvent GerarPedidoPagoEvent(PagamentoDb pagamentoDb) => new()
        {
            PedidoId = pagamentoDb.PedidoId,
            Status = pagamentoDb.Status
        };

        private static PedidoPendentePagamentoEvent GerarPedidoPendentePagamentoEvent(PagamentoDb pagamentoDb) => new()
        {
            PedidoId = pagamentoDb.PedidoId,
            Status = "PendentePagamento"
        };

        public static Pagamento ToPagamento(PagamentoDb pagamentoDb)
        {
            var statusPagamento = (StatusPagamento)Enum.Parse(typeof(StatusPagamento), pagamentoDb.Status, ignoreCase: true);

            return new Pagamento(pagamentoDb.Id, pagamentoDb.PedidoId, statusPagamento, pagamentoDb.Valor, pagamentoDb.QrCodePix, pagamentoDb.DataPagamento);
        }
    }
}
