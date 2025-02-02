using Amazon.SQS;
using Amazon.SQS.Model;
using Core.Infra.MessageBroker;
using Moq;
using System.Text.Json;

namespace Framepack.Worker.Tets.Core.Infra.MessageBroker;

public class SqsServiceTests
{
    private readonly Mock<IAmazonSQS> _sqsClientMock;
    private readonly SqsService<TestObject> _sqsService;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/MyQueue";

    public SqsServiceTests()
    {
        _sqsClientMock = new Mock<IAmazonSQS>();
        _sqsService = new SqsService<TestObject>(_sqsClientMock.Object, _queueUrl);
    }

    [Fact]
    public async Task SendMessageAsync_Success()
    {
        // Arrange
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var sendMessageResponse = new SendMessageResponse { MessageId = "12345" };

        _sqsClientMock.Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(sendMessageResponse);

        // Act
        var result = await _sqsService.SendMessageAsync(testObject);

        // Assert
        Assert.True(result);
        _sqsClientMock.Verify(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_Failure()
    {
        // Arrange
        var testObject = new TestObject { Id = 1, Name = "Test" };

        _sqsClientMock.Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((SendMessageResponse)null);

        // Act
        var result = await _sqsService.SendMessageAsync(testObject);

        // Assert
        Assert.False(result);
        _sqsClientMock.Verify(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReceiveMessagesAsync_Success()
    {
        // Arrange
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var messageBody = JsonSerializer.Serialize(testObject);
        var receiveMessageResponse = new ReceiveMessageResponse
        {
            Messages =
            [
                new Message { Body = messageBody, ReceiptHandle = "receipt-handle" }
            ]
        };

        _sqsClientMock.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(receiveMessageResponse);

        _sqsClientMock.Setup(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new DeleteMessageResponse());

        // Act
        var result = await _sqsService.ReceiveMessagesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testObject.Id, result.Id);
        Assert.Equal(testObject.Name, result.Name);
        _sqsClientMock.Verify(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _sqsClientMock.Verify(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReceiveMessagesAsync_JsonException()
    {
        // Arrange
        var invalidMessageBody = "invalid json";
        var receiveMessageResponse = new ReceiveMessageResponse
        {
            Messages =
            [
                new Message { Body = invalidMessageBody, ReceiptHandle = "receipt-handle" }
            ]
        };

        _sqsClientMock.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(receiveMessageResponse);

        // Act
        var result = await _sqsService.ReceiveMessagesAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
        _sqsClientMock.Verify(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _sqsClientMock.Verify(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}