using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
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
        var expectedResponse = _mockContext.Object.Consumers.Select(p => ConsumerMapper.MapEntityToResponse(p)).ToList();
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
        var ex = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex.Message);
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
    [Fact]
    public async void DeleteConsumers_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteConsumerCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.DeleteConsumer(id);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
}