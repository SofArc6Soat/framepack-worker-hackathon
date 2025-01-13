using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace UseCases.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddUseCasesDependencyServices(this IServiceCollection services) => services.AddScoped<IPagamentoUseCase, PagamentoUseCase>();
    }
}