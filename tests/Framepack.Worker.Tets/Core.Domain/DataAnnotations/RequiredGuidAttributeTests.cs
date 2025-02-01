using Core.Domain.DataAnnotations;

namespace Framepack.Worker.Tets.Core.Domain.DataAnnotations;

public class RequiredGuidAttributeTests
{
    private readonly RequiredGuidAttribute _attribute;

    public RequiredGuidAttributeTests()
    {
        _attribute = new RequiredGuidAttribute();
    }

    [Fact]
    public void IsValid_WithValidGuid_ReturnsTrue()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = _attribute.IsValid(validGuid);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithEmptyGuid_ReturnsFalse()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = _attribute.IsValid(emptyGuid);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithNullValue_ReturnsFalse()
    {
        // Act
        var result = _attribute.IsValid(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithNonGuidValue_ReturnsFalse()
    {
        // Arrange
        var nonGuidValue = "not-a-guid";

        // Act
        var result = _attribute.IsValid(nonGuidValue);

        // Assert
        Assert.False(result);
    }
}