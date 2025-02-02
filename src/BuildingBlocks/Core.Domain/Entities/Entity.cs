using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class Entity
    {
        public Guid Id { get; set; }

        protected static void Validar(ValidationResult results)
        {
            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    throw new DomainException(failure.ErrorMessage);
                }
            }
        }
    }
}