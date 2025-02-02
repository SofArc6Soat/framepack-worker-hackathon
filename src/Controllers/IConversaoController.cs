namespace Controllers
{
    public interface IConversaoController
    {
        Task<bool> ProcessarConversaoSolicitadaAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ProcessarDownloadEfetuadoAsync(Guid id, CancellationToken cancellationToken);
    }
}
