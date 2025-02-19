﻿using System.Diagnostics.CodeAnalysis;

namespace Worker.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class EnvironmentConfig
    {
        public static Settings ConfigureEnvironment(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new Settings();
            ConfigurationBinder.Bind(configuration, settings);

            return settings;
        }
    }

    [ExcludeFromCodeCoverage]
    public record Settings
    {
        public AwsDynamoDbSettings AwsDynamoDbSettings { get; set; } = new AwsDynamoDbSettings();

        public AwsSqsSettings AwsSqsSettings { get; set; } = new AwsSqsSettings();
    }

    [ExcludeFromCodeCoverage]
    public record AwsDynamoDbSettings
    {
        public string ServiceUrl { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public record AwsSqsSettings
    {
        public string QueueConversaoSolicitadaEvent { get; set; } = string.Empty;
        public string QueueDownloadEfetuadoEvent { get; set; } = string.Empty;
    }
}