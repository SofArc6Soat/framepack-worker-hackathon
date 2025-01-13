using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Core.WebApi.Controller
{
    [ExcludeFromCodeCoverage]
    public record BaseApiResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}