using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Queries.Services;
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
        var expectedResponse = new UpdatePasswordResponse(id, "Password updated successfully.");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.UpdatePassword(id, password);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<UpdatePasswordResponse>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
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
        var response = await _controller.UpdatePassword(id, password);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo AccountingClose Ok
    /// </summary>
    [Fact]
    public async void AccountingClose_Returns_Ok()
    {
        var providers = _mockContext.Object.Providers.Select(ProviderMapper.MapEntityToResponse).ToList();
        var expectedResponse = "Test response";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetLastAccountingCloseQuery>(), CancellationToken.None))
            .ReturnsAsync(DateTime.MinValue);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersWithServicesQuery>(), CancellationToken.None))
            .ReturnsAsync(providers);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetFieldsByServiceIdQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<FieldResponse>());
        foreach (var provider in providers)
        {
            foreach (var service in provider.Services!)
            {
                _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentsByServiceIdQuery>(q=>q.Id==service.Id), CancellationToken.None))
                    .ReturnsAsync(service.Payments);
            }
        }
        _mediatorMock.Setup(m => m.Send(It.IsAny<AccountingCloseRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        var response = await _controller.AccountingClose();
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo AccountingClose BadRequest
    /// </summary>
    [Fact]
    public async void AccountingClose_Returns_BadRequest()
    {
        var expectedException = new Exception("TestException");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetLastAccountingCloseQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.AccountingClose();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(expectedException.Message,badRequestResult.Value);
    }
}