using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ODGAPI.Controllers;
using Moq;
using Moq.Protected;

namespace ODGAPI_TDD;

public class Tests
{
    private PPPLoanController _controller = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<ILogger<PPPLoanController>> _mockLogger = null!;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<PPPLoanController>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

    var _httpClient = CreateMockHttpClient(_mockHttpMessageHandler.Object);
    _controller = new PPPLoanController(_mockConfiguration.Object, _mockLogger.Object, _httpClient);

    }

    private static HttpClient CreateMockHttpClient(HttpMessageHandler messageHandler)
    {
        return new HttpClient(messageHandler)
        {
            BaseAddress = new Uri("https://data.nj.gov/resource/")
        };
    }

    [Test]
    public void GetTestMessage_ShouldReturn_OkResult()
    {
        var result = _controller.GetTestMessage();

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        if(okResult != null)
        {
            Assert.AreEqual("Hello from MyNewController!", okResult.Value);
        }

    }
    [Test]
    public async Task GetPPPLoanDataAll_ShouldReturn_OkResult()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        };
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => true),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);
            _mockConfiguration.Setup(config => config["SodaApiKey"]).Returns("YourSodaApiKey");
            _mockConfiguration.Setup(config => config["PPPLoanURL"]).Returns("https://data.nj.gov/resource/");
            _mockConfiguration.Setup(config => config["PPPLoanResourceId"]).Returns("riep-z5cp");

        // Act
        var result = await _controller.GetPPPLoanDataAll();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }
    [Test]
    public async Task GetPPPLoanPaginatedData_ShouldReturn_OkResult()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => true),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        _mockConfiguration.Setup(config => config["SodaApiKey"]).Returns("YourSodaApiKey");
        _mockConfiguration.Setup(config => config["PPPLoanURL"]).Returns("https://data.nj.gov/resource/");
        _mockConfiguration.Setup(config => config["PPPLoanResourceId"]).Returns("riep-z5cp");

        // Act
        var result = await _controller.GetPPPLoanPaginatedData(1, 10);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }
}
