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
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de metodo get para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetProviders_Returns_Ok()
    {
        var expectedResponse = DataSeed.DataSeed.mockSetProviderEntity.Object.Select(p => ProviderMapper.MapEntityToResponse(p)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<ProvidersQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.GetProviders();
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<ProviderResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo get para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetProviders_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<ProvidersQuery>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.GetProviders();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<Exception>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex.Message);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void PostProviders_Returns_Ok()
    {
        var provider = new ProviderRequest() {Name = "Test Provider"};
        var expectedResponse = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProviderCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.PostProvider(provider);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void PostProviders_Returns_BadRequest()
    {
        var provider = new ProviderRequest() {Name = "Test Provider"};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProviderCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.PostProvider(provider);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
}