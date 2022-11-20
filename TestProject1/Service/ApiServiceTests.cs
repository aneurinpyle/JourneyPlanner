using Moq;
using Moq.Protected;
using JourneyPlannerClient.Common;
using JourneyPlannerClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JourneyPlannerClient.Test.Service
{
    [TestFixture]
    public class ApiServiceTests
    {
        private Dictionary<string, string> _tflMockQueryParams;
        private string _tflMockJourneyPlannerApiUrl;

        [SetUp]
        public void Setup()
        {
            _tflMockJourneyPlannerApiUrl = "https://mockTflApiUrl";
            _tflMockQueryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                };
        }

        [Test]
        public async Task GetAsync_WhenInvoked_TheCorrectURLAndQueryParamsSent()
        {
            //Arrange
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "key", "value" } })
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse)
                .Verifiable();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var service = new ApiServiceBuilder().WithHttpClientFactory(mockHttpClientFactory.Object).Build();

            //Act
            var response = await service.GetAsync(_tflMockJourneyPlannerApiUrl, _tflMockQueryParams);
            var expectedContent = await expectedResponse.Content.ReadAsStringAsync();
            var actualContent = await response.Content.ReadAsStringAsync();

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(ex => ex.Method == HttpMethod.Get && (expectedContent == actualContent)),
                ItExpr.IsAny<CancellationToken>()
            );
        }
        [Test]
        public void GetAsync_WhenRequestExceptionIsThrown_ThrowsRequestException()
        {
            //Arrange
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "key", "value" } })
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new HttpRequestException());
            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var service = new ApiServiceBuilder().WithHttpClientFactory(mockHttpClientFactory.Object).Build();

            //Act
            //Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await service.GetAsync(_tflMockJourneyPlannerApiUrl, _tflMockQueryParams));
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        public class ApiServiceBuilder
        {
            private IHttpClientFactory _clientFactory;

            internal ApiServiceBuilder()
            {
                _clientFactory = Mock.Of<IHttpClientFactory>();
            }

            internal ApiServiceBuilder WithHttpClientFactory(IHttpClientFactory clientFactory)
            {
                _clientFactory = clientFactory;
                return this;
            }

            internal ApiService Build()
            {
                return new ApiService(_clientFactory);
            }
        }
    }
}
