using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class ProvidersControllerTest
{
    private readonly ProvidersController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ProvidersController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public ProvidersControllerTest()
    {
        _loggerMock = new Mock<ILogger<ProvidersController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProvidersController(_loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
    }

    [Fact]
    public async void GetProviders_Returns_Ok()
    {
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        var expectedResponse = DataSeed.DataSeed.mockSetProviderEntity.Object.Select(p => ProviderMapper.MapEntityToResponse(p)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<ProvidersQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.GetProviders();
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<ProviderResponse>>(okResult.Value);
        Assert.Equal(okResult.Value, expectedResponse);
    }
}