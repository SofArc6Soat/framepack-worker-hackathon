using Core.WebApi.Configurations;

namespace Framepack.Worker.Tets.Core.WebApi.Configurations;

public class JwtBearerConfigureOptionsTests
{
    [Fact]
    public void JwtBearerConfigureOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new JwtBearerConfigureOptions();

        // Assert
        Assert.Equal(string.Empty, options.Authority);
        Assert.Equal(string.Empty, options.MetadataAddress);
    }

    [Fact]
    public void JwtBearerConfigureOptions_ShouldSetAuthority()
    {
        // Arrange
        var expectedAuthority = "https://example.com";

        // Act
        var options = new JwtBearerConfigureOptions
        {
            Authority = expectedAuthority
        };

        // Assert
        Assert.Equal(expectedAuthority, options.Authority);
    }

    [Fact]
    public void JwtBearerConfigureOptions_ShouldSetMetadataAddress()
    {
        // Arrange
        var expectedMetadataAddress = "https://example.com/.well-known/openid-configuration";

        // Act
        var options = new JwtBearerConfigureOptions
        {
            MetadataAddress = expectedMetadataAddress
        };

        // Assert
        Assert.Equal(expectedMetadataAddress, options.MetadataAddress);
    }

    [Fact]
    public void JwtBearerConfigureOptions_ShouldThrowException_WhenAuthorityIsNull()
    {
        // Arrange
        var options = new JwtBearerConfigureOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.Authority = null);
    }

    [Fact]
    public void JwtBearerConfigureOptions_ShouldThrowException_WhenMetadataAddressIsNull()
    {
        // Arrange
        var options = new JwtBearerConfigureOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.MetadataAddress = null);
    }
}
