using GenericRestHelper.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GenericRestHelper.Tests
{
    public class RestClientServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly Mock<ILogger<RestClientService>> _loggerMock;
        private readonly HttpClient _httpClient;
        private readonly RestClientService _service;
        private const string BaseUrl = "https://api.test.com/posts";

        public RestClientServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<RestClientService>>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new RestClientService(_httpClient, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsData_WhenResponseIsSuccess()
        {
            // Arrange
            var expectedData = new TestDto { Id = 1, Title = "Test" };
            var jsonResponse = JsonSerializer.Serialize(expectedData);
            SetupMockResponse(HttpStatusCode.OK, jsonResponse);

            // Act
            var result = await _service.GetAsync<TestDto>(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Title, result.Title);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_AndLogsError_WhenApiFails()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.NotFound, "Not Found");

            // Act
            var result = await _service.GetAsync<TestDto>(BaseUrl);

            // Assert
            Assert.Null(result);
            VerifyLoggerCalled("API Error", LogLevel.Error);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedObject_WhenSuccessful()
        {
            // Arrange
            var input = new TestDto { Title = "New Post" };
            var output = new TestDto { Id = 101, Title = "New Post" };
            SetupMockResponse(HttpStatusCode.Created, JsonSerializer.Serialize(output));

            // Act
            var result = await _service.PostAsync<TestDto, TestDto>(BaseUrl, input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(101, result.Id);
        }


        [Fact]
        public async Task PutAsync_ReturnsUpdatedObject_WhenSuccessful()
        {
            // Arrange
            var updateData = new TestDto { Id = 1, Title = "Updated" };
            SetupMockResponse(HttpStatusCode.OK, JsonSerializer.Serialize(updateData));

            // Act
            var result = await _service.PutAsync<TestDto, TestDto>($"{BaseUrl}/1", updateData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
        }


        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.NoContent, "");

            // Act
            await _service.DeleteAsync($"{BaseUrl}/1");

            // Assert
            _loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }


        private void SetupMockResponse(HttpStatusCode statusCode, string content)
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });
        }

        private void VerifyLoggerCalled(string expectedMessage, LogLevel level)
        {
            _loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }

    public class TestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}