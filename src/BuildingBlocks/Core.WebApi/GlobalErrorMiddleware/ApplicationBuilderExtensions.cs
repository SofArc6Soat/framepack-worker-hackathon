using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Core.WebApi.GlobalErrorMiddleware
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static void UseApplicationErrorMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();
    }
}