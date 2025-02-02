using System.ComponentModel.DataAnnotations;

namespace Core.Domain.DataAnnotations
{
    public class RequiredGuidAttribute : ValidationAttribute
    {
        public RequiredGuidAttribute() => ErrorMessage = "{0} é obrigatório.";

#pragma warning disable CS8765
        public override bool IsValid(object value)
#pragma warning restore CS8765
            => value is Guid && !Guid.Empty.Equals(value);
    }
}
