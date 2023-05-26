using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

public class PaymentsControllerTest
{
    private readonly PaymentsController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<PaymentsController>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public PaymentsControllerTest()
    {
        _loggerMock = new Mock<ILogger<PaymentsController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new PaymentsController(_loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta Ok
    /// </summary>
    [Fact]
    public async void PostPayment_Returns_Ok()
    {
        var payment = new PaymentRequest() {Amount = 10};
        var expectedResponse = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePaymentCommand>(), CancellationToken.None)).ReturnsAsync(expectedResponse);
        var response = await _controller.PostPayment(payment);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(expectedResponse,okResult.Value);
    }
    
    /// <summary>
    ///     Prueba de metodo post para prestadores con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void PostPayment_Returns_BadRequest()
    {
        var payment = new PaymentRequest() {Amount = 10};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePaymentCommand>(), CancellationToken.None)).ThrowsAsync(expectedException);
        var response = await _controller.PostPayment(payment);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
}