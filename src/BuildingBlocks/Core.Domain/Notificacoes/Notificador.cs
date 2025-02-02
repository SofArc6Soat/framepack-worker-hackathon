namespace Core.Domain.Notificacoes
{
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notificacoes;

        public Notificador() =>
            _notificacoes = [];

        public void Handle(Notificacao notificacao) =>
            _notificacoes.Add(notificacao);

        public List<Notificacao> ObterNotificacoes() =>
            _notificacoes;

        public bool TemNotificacao() =>
            _notificacoes.Count != 0;
    }
}
