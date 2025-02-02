using UseCases;

namespace Controllers
{
    public class ConversaoController(IConversaoUseCase conversaoUseCase) : IConversaoController
    {
        public async Task<bool> ProcessarConversaoSolicitadaAsync(Guid id, CancellationToken cancellationToken) =>
            await conversaoUseCase.ProcessarConversaoSolicitadaAsync(id, cancellationToken);
        public async Task<bool> ProcessarDownloadEfetuadoAsync(Guid id, CancellationToken cancellationToken) =>
            await conversaoUseCase.ProcessarDownloadEfetuadoAsync(id, cancellationToken);
    }
}
