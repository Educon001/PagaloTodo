using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Providers;
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
        var expectedResponse = _mockContext.Object.Providers.Select(p => ProviderMapper.MapEntityToResponse(p)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
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
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersQuery>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.GetProviders();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo get para prestadores con servicios con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetProvidersWithServices_Returns_Ok()
    {
        var providers = _mockContext.Object.Providers.ToList();
        var services = _mockContext.Object.Services.ToList();
        providers[0].Services!.AddRange(services);
        var expectedResponse = providers.Select(p=>ProviderMapper.MapEntityToResponse(p)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersWithServicesQuery>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.GetProvidersWithServices();
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<ProviderResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo get para prestadores con servicios con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetProvidersWithServices_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersWithServicesQuery>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.GetProvidersWithServices();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
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
    
    /// <summary>
    ///     Prueba de metodo Delete para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void DeleteProviders_Returns_Ok()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProviderCommand>(), CancellationToken.None)).ReturnsAsync(id);
        var response = await _controller.DeleteProvider(id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(id,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo Delete para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void DeleteProviders_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProviderCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.DeleteProvider(id);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo Update para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void UpdateProviders_Returns_Ok()
    {
        var id = Guid.NewGuid();
        var provider = new ProviderRequest() {Name = "Test Provider"};
        var expectedResponse = ProviderMapper.MapEntityToResponse(ProviderMapper.MapRequestToEntity(provider));
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProviderCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.UpdateProvider(id,provider);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<ProviderResponse>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo Update para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void UpdateProviders_Returns_BadRequest()
    {
        var id = Guid.NewGuid();
        var provider = new ProviderRequest() {Name = "Test Provider"};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProviderCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.UpdateProvider(id,provider);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
}