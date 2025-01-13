using System.Diagnostics.CodeAnalysis;

namespace Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract record Event
    {
        public Guid Id { get; set; }
    }
}
