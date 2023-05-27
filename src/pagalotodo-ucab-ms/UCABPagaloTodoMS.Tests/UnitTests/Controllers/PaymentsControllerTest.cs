using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Enums;
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
    ///     Prueba de metodo get para pagos con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPayments_Returns_Ok()
    {
        var expectedResponse = _mockContext.Object.Payments
            .Select(p => PaymentMapper.MapEntityToResponse(p, false, false)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.GetPayments(null, null);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<PaymentResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetPayments_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetPayments(null, null);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por consumidor respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentsByConsumerId_Returns_Ok()
    {
        var consumer = _mockContext.Object.Consumers.First();
        var expectedResponse =
            consumer.Payments?.Select(p => PaymentMapper.MapEntityToResponse(p, false, true)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsByConsumerIdQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.GetPaymentsByConsumerId(consumer.Id, null, null);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<PaymentResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por consumidor con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetPaymentsByConsumerId_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsByConsumerIdQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetPaymentsByConsumerId(new Guid(), null, null);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por prestador con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentsByProviderId_Returns_Ok()
    {
        var provider = _mockContext.Object.Providers.First();
        var expectedServiceResponse =
            provider.Services?.Select(s => ServiceMapper.MapEntityToResponse(s, true)).ToList();
        var expectedResponse = expectedServiceResponse?.First().Payments;
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesByProviderIdQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedServiceResponse);
        var response = await _controller.GetPaymentsByProviderId(provider.Id, null, null);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<PaymentResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por prestador con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetPaymentsByProviderId_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesByProviderIdQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetPaymentsByProviderId(new Guid(),null, null);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por servicio con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentsByServiceId_Returns_Ok()
    {
        var service = _mockContext.Object.Services.First();
        var expectedResponse =
            service.Payments?.Select(p => PaymentMapper.MapEntityToResponse(p, true, false)).ToList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsByServiceIdQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.GetPaymentsByServiceId(service.Id, null, null);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<PaymentResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo get para pagos por servicio con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void GetPaymentsByServiceId_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentsByServiceIdQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetPaymentsByServiceId(new Guid(), null, null);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo post para pagos con respuesta Ok
    /// </summary>
    [Fact]
    public async void PostPayment_Returns_Ok()
    {
        var payment = new PaymentRequest() {Amount = 10};
        var expectedResponse = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePaymentCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.PostPayment(payment);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo post para pagos con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void PostPayment_Returns_BadRequest()
    {
        var payment = new PaymentRequest() {Amount = 10};
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePaymentCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.PostPayment(payment);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
    
    /// <summary>
    ///     Prueba de metodo patch para pagos con respuesta Ok
    /// </summary>
    [Fact]
    public async void UpdatePaymentStatus_Returns_ok()
    {
        var payment = _mockContext.Object.Payments.First();
        var status = PaymentStatusEnum.Rechazado;
        var expectedResponse = payment.Id+" PaymentStatus updated successfully";
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePaymentStatusCommand>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.UpdatePaymentStatus(payment.Id,status);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo patch para pagos con respuesta BadRequest
    /// </summary>
    [Fact]
    public async void UpdatePaymentStatus_Returns_BadRequest()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePaymentStatusCommand>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.UpdatePaymentStatus(new Guid(),null);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }
}