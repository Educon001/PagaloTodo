using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class AdminControllerTest
{
    private readonly AdminsController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<AdminsController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public AdminControllerTest()
    {
        _loggerMock = new Mock<ILogger<AdminsController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminsController(_loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de metodo UpdatePassword para administradores con respuesta Ok
    /// </summary>
    [Fact]
    public async void UpdatePasswordHashProviders_Returns_Ok()
    {
        var id = Guid.NewGuid();
        var password = new UpdatePasswordRequest() {PasswordHash = "Ab123456.r"};
        var expectedResponse = new UpdatePasswordResponse(id,"Password updated successfully.");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.UpdatePassword(id,password);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<UpdatePasswordResponse>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo UpdatePassword para administradores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void UpdatePasswordHashProviders_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var password = new UpdatePasswordRequest() {PasswordHash = "string"};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.UpdatePassword(id,password);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
}