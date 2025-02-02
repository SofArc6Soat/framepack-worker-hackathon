using Core.Domain.Base;
using Core.Domain.Notificacoes;
using Gateways;

namespace UseCases
{
    public class ConversaoUseCase(IConversaoGateway conversaoGateway, INotificador notificador) : BaseUseCase(notificador), IConversaoUseCase
    {
        public async Task<bool> ProcessarConversaoSolicitadaAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversao = await conversaoGateway.ObterConversaoAsync(id, cancellationToken);

            return conversao is null
                ? throw new KeyNotFoundException("Conversão não encontrada")
                : await conversaoGateway.EfetuarConversaoAsync(conversao, cancellationToken);
        }

        public async Task<bool> ProcessarDownloadEfetuadoAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversao = await conversaoGateway.ObterConversaoAsync(id, cancellationToken);

            return conversao is null
                ? throw new KeyNotFoundException("Conversão não encontrada")
                : await conversaoGateway.DownloadEfetuadoAsync(conversao, cancellationToken);
        }
    }
}
