using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infra.MessageBroker.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAwsSqsMessageBroker(this IServiceCollection services)
        {
            var awsOptions = new AWSOptions
            {
                Profile = "default",
                Region = Amazon.RegionEndpoint.USEast1
            };

            services.AddDefaultAWSOptions(awsOptions);

            services.AddAWSService<IAmazonSQS>();
        }
    }
}