using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Framepack.Worker.Tets.Core.Infra.MessageBroker.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApiDefautConfig_ShouldConfigureServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockCredentials = new Mock<AWSCredentials>();
        var regionEndpoint = Amazon.RegionEndpoint.USEast1; // Defina a região apropriada

        services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
        {
            var config = new AmazonCognitoIdentityProviderConfig
            {
                RegionEndpoint = regionEndpoint
            };
            return new AmazonCognitoIdentityProviderClient(mockCredentials.Object, config);
        });

        // Act
        // Chame o método que você está testando, por exemplo:
        // services.AddApiDefautConfig();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cognitoClient = serviceProvider.GetService<IAmazonCognitoIdentityProvider>();

        Assert.NotNull(cognitoClient);
    }
}