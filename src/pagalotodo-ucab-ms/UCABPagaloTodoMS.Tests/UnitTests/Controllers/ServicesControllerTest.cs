using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Tests.MockData;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class ServicesControllerTest
{
    private readonly ServicesController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ServicesController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public ServicesControllerTest()
    {
        _loggerMock = new Mock<ILogger<ServicesController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new ServicesController(_loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
    }

    [Fact]
    public async Task GetServices_Returns_OK()
    {
        var services = BuildDataContextFaker.BuildServicesList();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServicesQuery>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(services));

        var result = await _controller.GetServices();
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    [Fact]
    public async Task GetServices_Returns_Error()
    {
        var expectedException = new Exception("New Exception");
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServicesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.GetServices();
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
        
    }

    [Fact]
    public async Task DeleteProviders_Returns_OK()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteServiceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(id);
        var response = await _controller.DeleteService(id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal(id, okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
        _mediatorMock.Verify();
    }
    
    [Fact]
    public async Task DeleteProviders_Returns_Error()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteServiceCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
        var response = await _controller.DeleteService(id);
        var badResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Equal(400, badResult.StatusCode);
        _mediatorMock.Verify();
    }
}
