using Infra.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infra.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddInfraDependencyServices(this IServiceCollection services, string dynamoDbServiceUrl, string dynamoDbAccessKey, string dynamoDbSecretKey) =>
            DynamoDbConfig.Configure(services, dynamoDbServiceUrl, dynamoDbAccessKey, dynamoDbSecretKey);
    }
}