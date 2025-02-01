using Amazon.S3;
using Amazon.S3.Model;
using Core.Infra.S3;
using Moq;

namespace Framepack.Worker.Tets.Core.Infra.S3;

public class S3ServiceTests
{
    private readonly Mock<IAmazonS3> _s3ClientMock;
    private readonly S3Service _s3Service;

    public S3ServiceTests()
    {
        _s3ClientMock = new Mock<IAmazonS3>();
        _s3Service = new S3Service(_s3ClientMock.Object);
    }

    [Fact]
    public void GerarPreSignedUrl_Success()
    {
        // Arrange
        var key = "test-key";
        var url = "https://example.com";
        _s3ClientMock.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(url);

        // Act
        var result = _s3Service.GerarPreSignedUrl(key);

        // Assert
        Assert.Equal(url, result);
    }

    [Fact]
    public async Task DeletarArquivoAsync_Success()
    {
        // Arrange
        var key = "test-key";
        _s3ClientMock.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
            .ReturnsAsync(new DeleteObjectResponse());

        // Act
        await _s3Service.DeletarArquivoAsync(key);

        // Assert
        _s3ClientMock.Verify(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task DeletarArquivoAsync_Failure()
    {
        // Arrange
        var key = "test-key";
        _s3ClientMock.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
            .ThrowsAsync(new AmazonS3Exception("Error deleting file"));

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _s3Service.DeletarArquivoAsync(key));
    }
}