using Amazon.CognitoIdentityProvider;
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
        var mockCognitoClient = new Mock<IAmazonCognitoIdentityProvider>();

        services.AddSingleton(mockCognitoClient.Object);

        // Act

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cognitoClient = serviceProvider.GetService<IAmazonCognitoIdentityProvider>();

        Assert.NotNull(cognitoClient);
    }
}