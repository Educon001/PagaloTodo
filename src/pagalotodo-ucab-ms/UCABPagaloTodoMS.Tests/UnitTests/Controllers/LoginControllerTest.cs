using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class LoginControllerTest
{
    private readonly LoginController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<LoginController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IMemoryCache> _cacheMock;

    public LoginControllerTest()
    {
        _loggerMock = new Mock<ILogger<LoginController>>();
        _mediatorMock = new Mock<IMediator>();
        _cacheMock = new Mock<IMemoryCache>();
        _controller = new LoginController(_loggerMock.Object, _mediatorMock.Object,_cacheMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "test", PasswordHash = "Password.", UserType = "consumer" };
        var expectedResponse = new LoginResponse { Id = Guid.NewGuid(), UserType = "consumer", Token = "some.jwt.token" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _controller.Authenticate(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
    
        Assert.Equal(expectedResponse.Id, loginResponse.Id);
        Assert.Equal(expectedResponse.UserType, loginResponse.UserType);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "john.doe", PasswordHash = "invalidpassword", UserType = "consumer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync((LoginResponse)null);

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async Task Login_NullRequest_ReturnsBadRequestResult()
    {
        // Arrange
        LoginRequest loginRequest = null;

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task Login_NullUsername_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = null, PasswordHash = "password123", UserType = "consumer" };

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task PasswordHash_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "john.doe", PasswordHash = null, UserType = "consumer" };

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task Login_NullUserType_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "john.doe", PasswordHash = "password123", UserType = null };

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task Authenticate_ExceptionThrown_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "test", PasswordHash = "Password.", UserType = "consumer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Error de prueba"));

        // Act
        var response = await _controller.Authenticate(loginRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Contains("Se ha producido un error al autenticar al usuario.", badRequestResult.Value.ToString());
    }
    
}

