using Domain.Entities;

namespace Gateways
{
    public interface IConversaoGateway
    {
        Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken);
    }
}
