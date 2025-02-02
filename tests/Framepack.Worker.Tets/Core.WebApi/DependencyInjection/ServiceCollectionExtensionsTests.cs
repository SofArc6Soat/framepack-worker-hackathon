using Amazon.CognitoIdentityProvider;
using Core.Domain.Notificacoes;
using Core.WebApi.Configurations;
using Core.WebApi.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Framepack.Worker.Tets.Core.WebApi.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services;
    private readonly Mock<JwtBearerConfigureOptions> _jwtBearerConfigureOptionsMock;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
        _jwtBearerConfigureOptionsMock = new Mock<JwtBearerConfigureOptions>();
        _jwtBearerConfigureOptionsMock.SetupAllProperties();
    }

    [Fact]
    public void AddApiDefautConfig_ShouldConfigureServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockCognitoClient = new Mock<IAmazonCognitoIdentityProvider>();

        // Registra o mock no container de serviços
        services.AddSingleton(mockCognitoClient.Object);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cognitoClient = serviceProvider.GetService<IAmazonCognitoIdentityProvider>();

        Assert.NotNull(cognitoClient);
    }

    [Fact]
    public void AddWorkerDefautConfig_ShouldConfigureServicesCorrectly()
    {
        // Act
        _services.AddWorkerDefautConfig();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var notificador = serviceProvider.GetService<INotificador>();
        Assert.NotNull(notificador);

        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        Assert.NotNull(jsonOptions);
        Assert.Equal(JsonIgnoreCondition.WhenWritingNull, jsonOptions.Value.JsonSerializerOptions.DefaultIgnoreCondition);
        Assert.Equal(JsonNamingPolicy.CamelCase, jsonOptions.Value.JsonSerializerOptions.PropertyNamingPolicy);
    }
}