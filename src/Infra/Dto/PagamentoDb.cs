using Amazon.DynamoDBv2.DataModel;

namespace Infra.Dto
{
    [DynamoDBTable("Pagamentos")]
    public class PagamentoDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public Guid PedidoId { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; } = string.Empty;

        [DynamoDBProperty]
        public decimal Valor { get; set; }

        [DynamoDBProperty]
        public string? QrCodePix { get; set; }

        [DynamoDBProperty]
        public DateTime DataPagamento { get; set; }
    }
}
