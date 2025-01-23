using Amazon.DynamoDBv2.DataModel;

namespace Infra.Dto
{
    [DynamoDBTable("Conversoes")]
    public class ConversaoDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public Guid UsuarioId { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; } = string.Empty;

        [DynamoDBProperty]
        public DateTime Data { get; set; }

        [DynamoDBProperty]
        public string NomeArquivo { get; set; } = string.Empty;

        [DynamoDBProperty]
        public string UrlArquivoVideo { get; set; } = string.Empty;

        [DynamoDBProperty]
        public string UrlArquivoCompactado { get; set; } = string.Empty;
    }
}