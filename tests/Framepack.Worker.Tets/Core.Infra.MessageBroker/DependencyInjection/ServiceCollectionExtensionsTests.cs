using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Core.Infra.MessageBroker.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Framepack.Worker.Tets.Core.Infra.MessageBroker.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAwsSqsMessageBroker_WithNullServiceCollection_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services.AddAwsSqsMessageBroker());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddAwsSqsMessageBroker_WithValidServiceCollection_AddsAwsServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAwsSqsMessageBroker();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var awsOptions = serviceProvider.GetService<AWSOptions>();
        var sqsClient = serviceProvider.GetService<IAmazonSQS>();

        Assert.NotNull(awsOptions);
        Assert.Equal("default", awsOptions.Profile);
        Assert.Equal(Amazon.RegionEndpoint.USEast1, awsOptions.Region);
        Assert.NotNull(sqsClient);
    }
}