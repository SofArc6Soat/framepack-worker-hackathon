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

            if (conversao is null)
            {
                // TODO: Gerar evento de erro
            }

            // Fazer download
            // Converter
            // Compactar
            // Fazer upload

            return false;
        }
    }
}
