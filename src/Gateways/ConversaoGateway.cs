using Amazon.DynamoDBv2.DataModel;
using Domain.Entities;
using Domain.ValueObjects;
using Infra.Dto;

namespace Gateways
{
    public class ConversaoGateway(IDynamoDBContext repository) : IConversaoGateway
    {
        public async Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversaoDto = await repository.LoadAsync<ConversaoDb>(id, cancellationToken);

            if (conversaoDto is null)
            {
                return null;
            }

            var status = (Status)Enum.Parse(typeof(Status), conversaoDto.Status, ignoreCase: true);

            return new Conversao(conversaoDto.Id, conversaoDto.UsuarioId, conversaoDto.Data, status, conversaoDto.NomeArquivo, conversaoDto.UrlArquivoVideo);
        }
    }
}
