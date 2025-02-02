namespace UseCases
{
    public interface IConversaoUseCase
    {
        Task<bool> ProcessarConversaoSolicitadaAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> ProcessarDownloadEfetuadoAsync(Guid id, CancellationToken cancellationToken);
    }
}