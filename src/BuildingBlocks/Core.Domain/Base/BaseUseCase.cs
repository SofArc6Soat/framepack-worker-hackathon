using Core.Domain.Entities;
using Core.Domain.Notificacoes;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Domain.Base
{
    public abstract class BaseUseCase(INotificador notificador)
    {
        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string mensagem) =>
            notificador.Handle(new Notificacao(mensagem));

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid)
            {
                return true;
            }

            Notificar(validator);

            return false;
        }
    }
}
