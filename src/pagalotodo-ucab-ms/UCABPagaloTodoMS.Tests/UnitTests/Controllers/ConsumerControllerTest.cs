using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class ConsumersControllerTest
{
    private readonly ConsumersController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ConsumersController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public ConsumersControllerTest()
    {
        _loggerMock = new Mock<ILogger<ConsumersController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new ConsumersController(_loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de metodo get para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetConsumers_Returns_Ok()
    {
        var expectedResponse = _mockContext.Object.Consumers.Select(ConsumerMapper.MapEntityToResponse).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<ConsumersQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.GetConsumers();
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<ConsumerResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo get para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetConsumers_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<ConsumersQuery>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.GetConsumers();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void PostConsumers_Returns_Ok()
    {
        var consumer = new ConsumerRequest() {Name = "Test Consumer"};
        var expectedResponse = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateConsumerCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.PostConsumer(consumer);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void PostConsumers_Returns_BadRequest()
    {
        var consumer = new ConsumerRequest() {Name = "Test Consumer"};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateConsumerCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.PostConsumer(consumer);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo Update para consumidores con respuesta Ok
    /// </summary>
    [Fact]
    public async void UpdateConsumers_Returns_Ok()
    {
        var id = Guid.NewGuid();
        var consumer = new ConsumerRequest() {Name = "Test Consumer"};
        var expectedResponse = ConsumerMapper.MapEntityToResponse(ConsumerMapper.MapRequestToEntity(consumer));
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateConsumerCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.UpdateConsumer(id,consumer);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<ConsumerResponse>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo Update para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void UpdateConsumers_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var consumer = new ConsumerRequest() {Name = "Test Consumer"};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateConsumerCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.UpdateConsumer(id,consumer);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo UpdatePassword para consumidores con respuesta Ok
    /// </summary>
    [Fact]
    public async void UpdatePasswordHashConsumers_Returns_Ok()
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
    ///     Prueba de metodo UpdatePassword para consumidores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void UpdatePasswordHashConsumers_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var password = new UpdatePasswordRequest() {PasswordHash = "string"};
        var expectedException = new CustomException(new Exception("Test Exception"));
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.UpdatePassword(id, password);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo Delete para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void DeleteConsumers_Returns_Ok()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsumerCommand>(), CancellationToken.None)).ReturnsAsync(id);
        var response = await _controller.DeleteConsumer(id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(id,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo Delete para prestadores con respuesta BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void DeleteConsumers_Returns_BadRequest(Type exceptionType)
    {
        var id = Guid.NewGuid();
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsumerCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.DeleteConsumer(id);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo get para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetConsumerById_Returns_Ok()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = ConsumerMapper.MapEntityToResponse(entity);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsumerByIdQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.GetConsumerById(entity.Id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<ConsumerResponse>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo get para consumers con respuesta BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void GetConsumerById_Returns_BadRequest(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetConsumerByIdQuery>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.GetConsumerById(Guid.Empty);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    [Fact]
    public async Task GetConsumers_ReturnsBadRequest_OnCustomException()
    {
        // Arrange
        var expectedException = new Exception("Test exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<ConsumersQuery>(), default))
            .ThrowsAsync(new CustomException(expectedException));
    
        // Act
        var result = await _controller.GetConsumers();
    
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var value = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("Test exception", value);
    }
    
    [Fact]
    public async Task PostConsumer_ReturnsBadRequest_OnCustomException()
    {
        // Arrange
        var consumer = new ConsumerRequest() { Name = "Test Consumer" };
        var expectedException = new Exception("Test exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateConsumerCommand>(), CancellationToken.None))
            .ThrowsAsync(new CustomException(expectedException));
    
        // Act
        var result = await _controller.PostConsumer(consumer);
    
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var value = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("Test exception", value);
    }
    
    [Fact]
    public async Task UpdatePassword_ReturnsBadRequest_OnException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdatePasswordRequest() { PasswordHash = "new password" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Test exception"));
    
        // Act
        var result = await _controller.UpdatePassword(id, request);
    
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var value = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("Error al cambiar la clave del consumer.", value);
    }
    
    [Fact]
    public async Task UpdateConsumer_ReturnsBadRequest_OnCustomException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var consumer = new ConsumerRequest() { Name = "Test Consumer" };
        var expectedException = new CustomException(new Exception("Test exception"));
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateConsumerCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
    
        // Act
        var result = await _controller.UpdateConsumer(id, consumer);
    
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var value = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("Test exception", value);
    }

}