using System.Net;
using BbcTravelMS.Services.Implementation;
using BbcTravelMS.Settings;
using BbcTravelMS.Tests.MockData;
using BbcTravelMS.Utils.Exceptions;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RestSharp;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.Services.Implementation
{
    public class CoreServiceTest
    {

        private readonly CoreService _service;
        private readonly Mock<ILogger<CoreService>> _mockLogger;
        private readonly Mock<IRestClient> _restClientMock;

        public CoreServiceTest()
        {
            var faker = new Faker();
            var optionsMock =
                new Mock<IOptions<UrlServiceSettings>>();
            optionsMock.SetupGet(c => c.Value)
                .Returns(new UrlServiceSettings
                {
                    CoreMsBaseUrl = faker.Internet.Url(),
                    CoreMsApiKey = faker.Internet.Mac(),
                });

            var appSettingsMock =
              new Mock<IOptions<AppSettings>>();

            appSettingsMock.SetupGet(c => c.Value)
                .Returns(new AppSettings
                {
                    ApiUserName = faker.Lorem.Word(),
                    ProductCode = faker.Lorem.Word(),
                });

            _mockLogger = new Mock<ILogger<CoreService>>();
            _restClientMock = new Mock<IRestClient>();
            _service = new CoreService(optionsMock.Object, appSettingsMock.Object, _restClientMock.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnInfoTravelQuotation()
        {
            var request = BuildDataContextFaker.BuildTravelRatePanRequest();

            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = BuildDataContextFaker.BuildTravelRatePanContent()
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var response = await _service.GetTravelQuotation(request);
            Assert.NotNull(response);
        }


        [Fact]
        public async Task ShouldReturnThrowTravelQuotation()
        {
            var request = BuildDataContextFaker.BuildTravelRatePanRequest();

            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var ex = await Assert.ThrowsAsync<CorePricingException>(async () => await _service.GetTravelQuotation(request));
            Assert.IsType<CorePricingException>(ex);
        }


        [Fact]
        public async Task ShouldReturnInfoProductPlans()
        {
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = BuildDataContextFaker.BuildProductPlansContent()
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var response = await _service.GetProductPlans();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ShouldReturnThrowProductPlans()
        {
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var ex = await Assert.ThrowsAsync<CoreProductsException>(async () => await _service.GetProductPlans());
            Assert.IsType<CoreProductsException>(ex);
        }


        [Fact]
        public async Task ShouldReturnInfoCountries()
        {
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = BuildDataContextFaker.BuildCountriesContent()
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var response = await _service.GetCountries();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ShouldReturnThrowCountries()
        {
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var ex = await Assert.ThrowsAsync<CoreRegionsException>(async () => await _service.GetCountries());
            Assert.IsType<CoreRegionsException>(ex);
        }

        [Fact]
        public async Task ShouldReturnInfoRegionsCountry()
        {
            var countryCode = "001";
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = BuildDataContextFaker.BuildCountriesContent()
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var response = await _service.GetRegionsCountry(countryCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ShouldReturnThrowRegionsCountry()
        {
            var countryCode = "001";
            IRestResponse restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
            };

            _restClientMock.Setup(c => c.ExecuteAsync(
              It.IsAny<IRestRequest>(),
              It.IsAny<CancellationToken>())
            ).ReturnsAsync(restResponse);

            var ex = await Assert.ThrowsAsync<CoreRegionsException>(async () => await _service.GetRegionsCountry(countryCode));
            Assert.IsType<CoreRegionsException>(ex);
        }
    }
}
