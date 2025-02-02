namespace UseCases
{
    public interface IConversaoUseCase
    {
        Task<bool> ProcessarConversaoSolicitadaAsync(Guid id, CancellationToken cancellationToken);
    }
}