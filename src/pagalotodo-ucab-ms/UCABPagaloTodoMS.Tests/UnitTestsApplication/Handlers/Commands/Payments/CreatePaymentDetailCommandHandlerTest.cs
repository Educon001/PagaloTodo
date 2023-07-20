using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands.Payments;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Payments;

[ExcludeFromCodeCoverage]
public class CreatePaymentDetailCommandHandlerTest
{
    private readonly CreatePaymentDetailCommandHandler _handler;
    private readonly Mock<ILogger<CreatePaymentDetailCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreatePaymentDetailCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreatePaymentDetailCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreatePaymentDetailCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreatePaymentDetailCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var payment = _mockContext.Object.Payments.First();
        var request = new PaymentDetailRequest()
        {
            Name = "Test Field",
            Value = "Test",
            Payment = payment.Id
        };
        _mockContext.Setup(c => c.Payments.Find(request.Payment)).Returns(payment);
        _mockContext.Setup(c => c.PaymentDetails.Add(It.IsAny<PaymentDetailEntity>()))
            .Callback((PaymentDetailEntity e) => e.Id = expectedResponse);
        var command = new CreatePaymentDetailCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }
    
    [Fact]
    public async void CreatePaymentDetailCommandHandler_ArgumentNullException()
    {
        var command = new CreatePaymentDetailCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
    
    [Fact]
    public async void CreatePaymentDetailCommandHandler_HandleAsyncException()
    {
        var payment = _mockContext.Object.Payments.First();
        var request = new PaymentDetailRequest()
        {
            Name = "Test Field",
            Value = "Test",
            Payment = payment.Id
        };
        var command = new CreatePaymentDetailCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Contains(payment.Id.ToString(), result.Message);
    }
}