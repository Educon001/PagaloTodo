using BbcTravelMS.Controllers;
using BbcTravelMS.Entities;
using BbcTravelMS.Models.Starter;
using BbcTravelMS.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.Controllers;

public class StarterControllerTests
{
    private readonly Mock<ILogger<StarterController>> _loggerMock;

    private readonly Mock<IStarterService> _starterService;

    private StarterController _starterController;

    public StarterControllerTests()
    {
        _starterService = new Mock<IStarterService>();
        _loggerMock = new Mock<ILogger<StarterController>>();
        _starterController = new StarterController(_loggerMock.Object, _starterService.Object);
    }

    [Fact(DisplayName = "GetStarterList Should Return OK")]
    public async Task GetStarterListShouldReturnOk()
    {
        var list = new List<StarterEntity>();
        _starterService.Setup(c => c.GetStarterList())
            .Returns(Task.FromResult(list));

        var result = await _starterController.GetStarterList();

        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, response.StatusCode);
        _starterService.Verify();
    }


    [Fact(DisplayName = "GetStarterList Should Return Bad Request")]
    public async Task GetStarterListReturnBadRequest()
    {
        _starterService.Setup(c => c.GetStarterList())
            .Throws(new NullReferenceException());

        var result = await _starterController.GetStarterList();

        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _starterService.Verify();
    }


    [Theory(DisplayName = "CreateStarter Should Return OK or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateStarterShouldReturnOk(bool isValid)
    {
        var requestData = new StarterRequest { Id = Guid.NewGuid(), Name = isValid ? "rlasso" : null };

        var starterEntity = requestData;
        StarterEntity starter = new() { Id = starterEntity.Id, Name = starterEntity.Name };

        _starterService.Setup(c => c.CreateStarterAsync(starter))
            .Returns(Task.FromResult(new StarterEntity()));

        var result = await _starterController.CreateStarter(requestData);

        if (isValid)
        {
            var responses = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, responses.StatusCode);
            _starterService.Verify();
            return;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
    }

    [Theory(DisplayName = "CreateStarter Should Return Bad Request or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateStarterShouldBadRequest(bool isValid)
    {
        var requestData = new StarterRequest { Id = Guid.NewGuid(), Name = isValid ? "rlasso" : null };

        var starterEntity = requestData;
        StarterEntity starter = new() { Id = starterEntity.Id, Name = starterEntity.Name };

        _starterService.Setup(c => c.CreateStarterAsync(starter))
            .Throws(new NullReferenceException());

        var result = await _starterController.CreateStarter(isValid ? null : requestData);

        if (isValid)
        {
            var responses = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, responses.StatusCode);
            _starterService.Verify();
            return;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
    }


    [Theory(DisplayName = "PublishTopic Should Return OK or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PublishTopicShouldReturnOk(bool isValid)
    {
        var requestData = new StarterRequest { Id = Guid.NewGuid(), Name = isValid ? "rlasso" : null };

        _starterController = new StarterController(_loggerMock.Object, _starterService.Object);
        var result = await _starterController.PublishTopic(requestData);

        if (isValid)
        {
            var responses = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, responses.StatusCode);
            _starterService.Verify();
            return;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
    }


    [Theory(DisplayName = "PublishTopic Should Return Bad Request or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PublishTopicShouldBadRequest(bool isValid)
    {
        var requestData = new StarterRequest { Id = Guid.NewGuid(), Name = isValid ? "rlasso" : null };

        _starterController = new StarterController(_loggerMock.Object, _starterService.Object);
        var result = await _starterController.PublishTopic(isValid ? null : requestData);

        if (isValid)
        {
            var responses = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, responses.StatusCode);
            _starterService.Verify();
            return;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
    }

    [Theory(DisplayName = "CalculateAgeRate Should Return OK or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public Task CalculateAgeRateShouldReturnOk(bool isValid)
    {
        var requestData = new AgeRateRequest { BirthDay = isValid ? DateTime.Now : null };

        _starterService.Setup(c => c.CalculateAgeRate(requestData.BirthDay))
            .Returns(It.IsAny<int>());

        var result = _starterController.CalculateAgeRate(requestData);

        if (isValid)
        {
            var responses = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, responses.StatusCode);
            _starterService.Verify();
            return Task.CompletedTask;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
        return Task.CompletedTask;
    }

    [Theory(DisplayName = "CalculateAgeRate Should Return Bad Request or Invalid")]
    [InlineData(true)]
    [InlineData(false)]
    public Task CalculateAgeRateShouldBadRequest(bool isValid)
    {
        var requestData = new AgeRateRequest { BirthDay = isValid ? DateTime.Now : null };

        _starterService.Setup(c => c.CalculateAgeRate(requestData.BirthDay))
            .Throws(new NullReferenceException());

        var result = _starterController.CalculateAgeRate(isValid ? null : requestData);

        if (isValid)
        {
            var responses = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, responses.StatusCode);
            _starterService.Verify();
            return Task.CompletedTask;
        }

        var response = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
        Assert.Equal(422, response.StatusCode);
        _starterService.Verify();
        return Task.CompletedTask;
    }
}
