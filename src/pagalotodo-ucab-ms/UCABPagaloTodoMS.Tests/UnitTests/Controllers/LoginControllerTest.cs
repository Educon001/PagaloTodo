using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Queries.Consumers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Token;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class LoginControllerTest
{
    private readonly LoginController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<LoginController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<ICacheEntry> _cacheEntryMock;
    private readonly Mock<IUrlHelper> _urlHelperMock;

    public LoginControllerTest()
    {
        _loggerMock = new Mock<ILogger<LoginController>>();
        _mediatorMock = new Mock<IMediator>();
        _cacheMock = new Mock<IMemoryCache>();
        _cacheEntryMock = new Mock<ICacheEntry>();
        _urlHelperMock = new Mock<IUrlHelper>();
        _controller = new LoginController(_loggerMock.Object, _mediatorMock.Object, _cacheMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _controller.Url = _urlHelperMock.Object;
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente credenciales válidas.
    /// </summary>
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = "test", PasswordHash = "Password.", UserType = "consumer"};
        var result = new LoginResponse {UserType = "consumer", Id = Guid.NewGuid()};
        var expectedResponse = new LoginResponse
            {Id = result.Id, UserType = result.UserType, Token = GenerarToken.GenerateToken(result)};
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _controller.Authenticate(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.NotNull(loginResponse.Token);
        Assert.Equal(expectedResponse.Id, loginResponse.Id);
        Assert.Equal(expectedResponse.UserType, loginResponse.UserType);
    }

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente credenciales inválidas.
    /// </summary>
    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest
            {Username = "john.doe", PasswordHash = "invalidpassword", UserType = "consumer"};
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync((LoginResponse) null);

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        var actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente request nulos.
    /// </summary>
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

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente request con username null.
    /// </summary>
    [Fact]
    public async Task Login_NullUsername_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = null, PasswordHash = "password123", UserType = "consumer"};

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente request con PasswordHash null.
    /// </summary>
    [Fact]
    public async Task PasswordHash_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = "john.doe", PasswordHash = null, UserType = "consumer"};

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    /// <summary>
    ///     Prueba para verificar que el método Authenticate maneja correctamente request con UserType null.
    /// </summary>
    [Fact]
    public async Task Login_NullUserType_ReturnsBadRequestResult()
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = "john.doe", PasswordHash = "password123", UserType = null};

        // Act
        var result = await _controller.Authenticate(loginRequest);

        // Assert
        Assert.IsType<ActionResult<LoginResponse>>(result);
        Assert.IsType<BadRequestResult>(result.Result);
    }
    
    /// <summary>
    ///     Prueba para verificar que el método Authenticatedevuelve un resultado BadRequest cuando se produce una excepción al autenticar al usuario.
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception), "Se ha producido un error al autenticar al usuario.")]
    [InlineData(typeof(CustomException), "Test Exception")]
    [InlineData(typeof(HttpRequestException), "Test Exception")]
    public async Task Authenticate_ExceptionThrown_ReturnsBadRequestResult(Type exceptionType, string message)
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = "test", PasswordHash = "Password.", UserType = "consumer"};
        var expectedException = exceptionType==typeof(HttpRequestException) ? 
            new HttpRequestException(null, null, HttpStatusCode.BadRequest) 
            : (Exception)Activator.CreateInstance(exceptionType, message);
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);

        // Act
        var response = await _controller.Authenticate(loginRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Contains(expectedException!.Message, badRequestResult.Value!.ToString()!);
    }
    
    [Fact]
    public async Task Authenticate_ExceptionThrown_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest {Username = "test", PasswordHash = "Password.", UserType = "consumer"};
        var expectedException = new HttpRequestException("Test Exception", null, HttpStatusCode.Unauthorized);
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);

        // Act
        var response = await _controller.Authenticate(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<ObjectResult>(response.Result);
        Assert.Contains(expectedException!.Message, unauthorizedResult.Value!.ToString()!);
    }

    /// <summary>
    ///     Prueba de metodo de ForgotPassword con respuesta ok
    /// </summary>
    [Fact]
    public async void ForgotPassword_Returns_Ok()
    {
        var email = "prueba@prueba.com";
        var consumerId = Guid.NewGuid();
        var expectedResponse = new ForgotPasswordResponse(ForgotPasswordResponse.Successful);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetConsumerByEmailQuery>(), CancellationToken.None))
            .ReturnsAsync(consumerId);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ForgotPasswordRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        _cacheMock
            .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
            .Returns(_cacheEntryMock.Object);
        var response = await _controller.ForgotPassword(email);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<ForgotPasswordResponse>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo de ForgotPassword con respuesta badRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void ForgotPassword_Returns_BadRequest(Type exceptionType)
    {
        var email = "prueba@prueba.com";
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetConsumerByEmailQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.ForgotPassword(email);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo de ResetPassword con respuesta ok
    /// </summary>
    [Fact]
    public async void ResetPassword_Returns_Ok()
    {
        var token = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dictionary = new Dictionary<string, object>()
        {
            {"UserId", userId}
        };
        object dictionaryObject = dictionary;
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "NewPassword."
        };
        var expectedResponse = new UpdatePasswordResponse(userId, "Password updated successfully");
        _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out dictionaryObject))
            .Returns(true);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.ResetPassword(token, request);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<UpdatePasswordResponse>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo de ResetPassword con respuesta ok
    /// </summary>
    [Fact]
    public async void ResetPassword_Returns_BadRequest()
    {
        var token = Guid.NewGuid();
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "NewPassword."
        };
        var response = await _controller.ResetPassword(token, request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains(token.ToString(), ex);
    }
    
    [Fact]
    public async void ResetPassword_Returns_BadRequest_Exception()
    {
        var token = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dictionary = new Dictionary<string, object>()
        {
            {"UserId", userId}
        };
        object dictionaryObject = dictionary;
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "NewPassword."
        };
        var expectedException = new Exception("Test Exception");
        _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out dictionaryObject))
            .Returns(true);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.ResetPassword(token, request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(expectedException.Message, ex);
    }
}