using JourneyPlannerClient.Common;
using JourneyPlannerClient.Model;
using JourneyPlannerClient.Service;
using Moq;
using NUnit.Framework.Constraints;
using System.Net;

namespace JourneyPlannerClient.Test.Service
{
    public class JourneyPlannerServiceTests
    {
        private string _journeyPlannerApiResponse;
        private string _stopPointApiResponse;
        private ApplicationConfiguration _applicationConfig;
        private Dictionary<string, string> _queryParams;

        [OneTimeSetUp]
        public void SetUp()
        {
            _journeyPlannerApiResponse = File.ReadAllText($"./TestData/TfLJourneyPlannerApiMockResponse.json");
            _stopPointApiResponse = File.ReadAllText($"./TestData/TfLStopPointApiMockResponse.json");

            _applicationConfig = new ApplicationConfiguration { TflApiKey = "123456", TflJourneyPlannerApiUrl = "https://mockTflJPApiUrl", TflStopPointApiUrl = "https://mockTflStopPointApiUrl" };

            _queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                    {Constants.Modes, Constants.Tube},
                };
        }

        [Test]
        public async Task GetJourneyPlannerFromApiAsync_WhenInvoked_ReturnsValidResponse()
        {
            //Arrange
            var startStation = "1000075";
            var destinationStation = "1000135";

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_journeyPlannerApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            //Act
            var result = await journeyPlannerService.GetJourneyFromApiAsync(startStation, destinationStation, null, null);

            //Assert
            mockApiService.Verify(x => x.GetAsync("https://mockTflJPApiUrl/1000075/to/1000135", _queryParams), Times.Once);
            Assert.IsInstanceOf<IEnumerable<Journeys>>(result);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetJourneyPlannerFromApiAsync_WhenErrorTooManyRequests_ThrowsArgumentException()
        {
            //Arrange
            var startStation = "1000075";
            var destinationStation = "1000135";

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.TooManyRequests
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => journeyPlannerService.GetJourneyFromApiAsync(startStation, destinationStation, null, null));

            Assert.That(ex.Message, Is.EqualTo("Error: Too many API requests and/or App Key supplied is invalid and may have expired and/or the API URL is invalid. Please check the status of your App Key and the API URL on the TfL API Developer Portal."));
            mockApiService.Verify(x => x.GetAsync("https://mockTflJPApiUrl/1000075/to/1000135", _queryParams), Times.Once);
        }

        [Test]
        public void GetJourneyPlannerFromApiAsync_WhenInternalServerError_ThrowsException()
        {
            //Arrange
            var startStation = "1000075";
            var destinationStation = "1000135";

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.InternalServerError
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => journeyPlannerService.GetJourneyFromApiAsync(startStation, destinationStation, null, null));

            Assert.That(ex.Message, Is.EqualTo("Error: API request failed. Reason: Internal Server Error. ErrorCode: 500."));
            mockApiService.Verify(x => x.GetAsync("https://mockTflJPApiUrl/1000075/to/1000135", _queryParams), Times.Once);
        }

        [Test]
        public async Task GetStopPointFromApiAsync_WhenInvoked_ReturnsValidResponse()
        {
            //Arrange
            var stationQuery = "Temple";
            var queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                    {Constants.Modes, Constants.Tube},
                    {Constants.Query, stationQuery }
                };

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_stopPointApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var applicationConfig = new ApplicationConfiguration { TflApiKey = "123456", TflStopPointApiUrl = "https://mockTflStopPointApiUrl/" };

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, applicationConfig);

            //Act
            var result = await journeyPlannerService.GetStopPointFromApiAsync(stationQuery);

            //Assert
            Assert.IsInstanceOf<string>(result);
            Assert.That(result, Is.EqualTo("940GZZLUTMP"));
        }

        [Test]
        public void GetStopPointFromApiAsync_WhenErrorTooManyRequests_ThrowsArgumentException()
        {
            //Arrange
            var stationQuery = "Temple";
            var queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                    {Constants.Modes, Constants.Tube},
                    {Constants.Query, stationQuery }
                };

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.TooManyRequests
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => journeyPlannerService.GetStopPointFromApiAsync(stationQuery));

            Assert.That(ex.Message, Is.EqualTo("Error: Too many API requests and/or App Key supplied is invalid and may have expired and/or the API URL is invalid. Please check the status of your App Key and the API URL on the TfL API Developer Portal."));
        }

        [Test]
        public void GetStopPointFromApiAsync_WhenInternalServerError_ThrowsException()
        {
            //Arrange
            var stationQuery = "Temple";
            _queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                    {Constants.Modes, Constants.Tube},
                    {Constants.Query, stationQuery }
                };

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.InternalServerError
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => journeyPlannerService.GetStopPointFromApiAsync(stationQuery));

            Assert.That(ex.Message, Is.EqualTo("Error: API request failed. Reason: Internal Server Error. ErrorCode: 500."));
        }

        [Test]
        public async Task ReturnJourneyInformation_WhenStartDestinationGiven_ReturnedInConsole()
        {
            //Arrange
            var startStation = "Embankment";
            var destinationStation = "Leicester Square";

            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_journeyPlannerApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var journeyPlannerService = new JourneyPlannerService(mockApiService.Object, _applicationConfig);

            var expectedOutput = "Your journey from Embankment to Leicester Square";
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            //Act
            await journeyPlannerService.ReturnJourneyInformation(startStation, destinationStation, null, null);

            //Asserts
            Assert.That(consoleOutput.ToString(), Does.StartWith(expectedOutput));
        }
    }
}
