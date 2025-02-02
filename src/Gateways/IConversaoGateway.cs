using Domain.Entities;

namespace Gateways
{
    public interface IConversaoGateway
    {
        Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> EfetuarConversaoAsync(Conversao conversao, CancellationToken cancellationToken);
    }
}
