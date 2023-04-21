using BbcTravelMS.Database;
using BbcTravelMS.Requests;
using BbcTravelMS.Services.Implementation;
using BbcTravelMS.Services.Interface;
using BbcTravelMS.Settings;
using BbcTravelMS.Tests.DataSeed;
using BbcTravelMS.Tests.MockData;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NASSA.CoreModels.Models.Pricing.PersonalAccidents;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.Services.Implementation
{
    public class TravelServiceTests
    {
        private readonly TravelService _service;
        private readonly Mock<IBbcTravelDbContext> _contextMock;
        private readonly Mock<ILogger<TravelService>> _mockLogger;
        private readonly Mock<ICoreService> _coreServiceMock;

        public TravelServiceTests()
        {
            var faker = new Faker();
            _contextMock = new Mock<IBbcTravelDbContext>();

            var appSettingsMock =
              new Mock<IOptions<AppSettings>>();

            appSettingsMock.SetupGet(c => c.Value)
                .Returns(new AppSettings
                {
                    ApiUserName = faker.Lorem.Word(),
                    ProductCode = faker.Lorem.Word(),
                });

            _mockLogger = new Mock<ILogger<TravelService>>();
            _coreServiceMock = new Mock<ICoreService>();
            _service = new TravelService(_contextMock.Object, _coreServiceMock.Object, appSettingsMock.Object, _mockLogger.Object);
            _contextMock.SetupDbContextData();
        }


        [Fact]
        public async Task ShouldCreateNewQuotation()
        {
            var request = BuildDataContextFaker.BuildQuotationRequest();
            var proxy = new Mock<IDbContextTransactionProxy>();
            _contextMock.Setup(x => x.BeginTransaction()).Returns(proxy.Object);
            var result = await _service.CreateQuotation(request);
            Assert.NotNull(result);
        }


        [Fact]
        public async Task ShouldCreateNewQuotationBadRequest()
        {
            var proxy = new Mock<IDbContextTransactionProxy>();
            _contextMock.Setup(x => x.BeginTransaction()).Returns(proxy.Object);
            var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateQuotation(null));
            Assert.IsType<NullReferenceException>(ex);
        }


        [Fact]
        public async Task ShouldReturnInfoTravelPlansRequestUnder70()
        {
            var request = BuildDataContextFaker.BuildTravelPlanRequestUnder70();

            MockGetProductPlans();
            MockGetTravelQuotation();

            var result = await _service.GetTravelPlans(request);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldReturnInfoTravelPlansRequestOver70()
        {
            var request = BuildDataContextFaker.BuildTravelPlanRequestOver70();

            MockGetProductPlans();
            MockGetTravelQuotation();

            var result = await _service.GetTravelPlans(request);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldReturnInfoTravelPlansRequestToEuropeOver70()
        {
            var request = BuildDataContextFaker.BuildTravelPlanRequestToEuropeOver70();

            MockGetProductPlans();
            MockGetTravelQuotation();

            var result = await _service.GetTravelPlans(request);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldReturnInfoTravelPlansRequestToEuropeUnder70()
        {
            var request = BuildDataContextFaker.BuildTravelPlanRequestToEuropeUnder70();

            MockGetProductPlans();
            MockGetTravelQuotation();

            var result = await _service.GetTravelPlans(request);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldThrowInfoTravelPlansRequest()
        {
            var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetTravelPlans(new TravelPlansRequest()));
            Assert.IsType<NullReferenceException>(ex);
        }

        private void MockGetProductPlans()
        {
            _coreServiceMock.Setup(p => p.GetProductPlans()).ReturnsAsync(new Utils.ProductModel.Result()
            {
                Data = BuildDataContextFaker.BuildProductModelData()
            });
        }

        private void MockGetTravelQuotation()
        {
            _coreServiceMock.Setup(p => p.GetTravelQuotation(It.IsAny<TravelRatePanRequest>())).ReturnsAsync(new Utils.TravelRateModel.Result()
            {
                Data = BuildDataContextFaker.BuildTravelRateModelData()
            });
        }

        [Fact]
        public async Task ShouldReturnInfoCountries()
        {
            var result = await _service.GetCountries();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldThrowInfoCountries()
        {
            _contextMock.Setup(x => x.Countries).Callback(() => throw new Exception("A test exception"));
            var ex = await Assert.ThrowsAsync<Exception>(async () => await _service.GetCountries());
            Assert.IsType<Exception>(ex);
            Assert.Contains("A test exception", ex.Message);
        }

        [Fact]
        public async Task ShouldUpdateInfoQuotation()
        {
            var request = BuildDataContextFaker.BuildQuotationUpdateRequest();
            var proxy = new Mock<IDbContextTransactionProxy>();
            _contextMock.Setup(x => x.BeginTransaction()).Returns(proxy.Object);
            var result = await _service.UpdateQuotation(request);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldThrowUpdateInfoQuotation()
        {
            var proxy = new Mock<IDbContextTransactionProxy>();
            _contextMock.Setup(x => x.BeginTransaction()).Returns(proxy.Object);
            var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.UpdateQuotation(null));
            Assert.IsType<NullReferenceException>(ex);
        }

        [Fact]
        public async Task ShouldReturnInfoCountryRegions()
        {
            var countryCode = "101";

            _coreServiceMock.Setup(p => p.GetRegionsCountry(countryCode)).ReturnsAsync(new Utils.RegionsModel.Result());
            var result = await _service.GetCountryRegions(countryCode);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldThrowInfoCountryRegions()
        {
            var countryCode = "101";
            _coreServiceMock.Setup(p => p.GetRegionsCountry(It.IsAny<string>())).ThrowsAsync(new Exception());
            var ex = await Assert.ThrowsAsync<Exception>(async () => await _service.GetCountryRegions(countryCode));
            Assert.IsType<Exception>(ex);
        }
    }
}
