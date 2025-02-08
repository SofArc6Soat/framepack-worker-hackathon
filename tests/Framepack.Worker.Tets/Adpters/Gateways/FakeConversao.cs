using Domain.Entities;
using Domain.ValueObjects;

namespace Framepack.Worker.Tets.Adpters.Gateways
{
    public class FakeConversao(
        Guid id,
        string usuarioId,
        DateTime data,
        Status status,
        string nomeArquivo,
        string urlArquivoVideo) : Conversao(id, usuarioId, data, status, nomeArquivo, urlArquivoVideo)
    {
        public bool RetornoAlterarStatusProcessando { get; set; } = true;

        public bool RetornoAlterarStatusCompactando { get; set; } = true;

        public new bool AlterarStatusParaProcessando(Status status) => RetornoAlterarStatusProcessando;

        public new bool AlterarStatusParaCompactando(Status status) => RetornoAlterarStatusCompactando;
    }
}
