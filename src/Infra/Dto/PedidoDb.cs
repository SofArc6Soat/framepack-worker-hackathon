using Amazon.DynamoDBv2.DataModel;

namespace Infra.Dto
{
    [DynamoDBTable("Pedidos")]
    public class PedidoDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public int NumeroPedido { get; set; }

        [DynamoDBProperty]
        public decimal ValorTotal { get; set; }

        [DynamoDBProperty]
        public DateTime DataPedido { get; set; }
    }
}
