using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Core.Infra.S3.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddAwsS3(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<IS3Service, S3Service>();
        }
    }
}