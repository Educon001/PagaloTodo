using System.Diagnostics.CodeAnalysis;
using FluentValidation;
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
public class CreatePaymentCommandHandlerTest
{
    private readonly CreatePaymentCommandHandler _handler;
    private readonly Mock<ILogger<CreatePaymentCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreatePaymentCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreatePaymentCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreatePaymentCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreatePaymentCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var entity = _mockContext.Object.Payments.Last();
        var request = new PaymentRequest()
        {
            PaymentStatus = entity.PaymentStatus,
            Amount = entity.Amount,
            Consumer = entity.Consumer!.Id,
            Identifier = entity.Identifier,
            Service = entity.Service!.Id,
            CardholderName = entity.CardholderName,
            CardNumber = entity.CardNumber,
            ExpirationMonth = entity.ExpirationMonth,
            ExpirationYear = entity.ExpirationYear,
            CardSecurityCode = entity.CardSecurityCode
        };
        _mockContext.Setup(c => c.Services.Find(entity.Service.Id)).Returns(entity.Service);
        _mockContext.Setup(c => c.Consumers.Find(entity.Consumer.Id)).Returns(entity.Consumer);
        _mockContext.Setup(c => c.Payments.Add(It.IsAny<PaymentEntity>()))
            .Callback((PaymentEntity p) => p.Id = expectedResponse);
        var command = new CreatePaymentCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void CreatePaymentCommandHandle_ArgumentNullException()
    {
        var command = new CreatePaymentCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }

    [Fact]
    public async void CreatePaymentCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Payments.First();
        var request = new PaymentRequest()
        {
            PaymentStatus = entity.PaymentStatus,
            Amount = entity.Amount,
            Consumer = entity.Consumer!.Id,
            Identifier = entity.Identifier,
            Service = entity.Service!.Id,
            CardholderName = entity.CardholderName,
            CardNumber = entity.CardNumber,
            ExpirationMonth = entity.ExpirationMonth,
            ExpirationYear = entity.ExpirationYear,
            CardSecurityCode = entity.CardSecurityCode
        };
        var command = new CreatePaymentCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Contains(entity.Service!.Id.ToString(), result.Message);
        Assert.Contains(entity.Consumer!.Id.ToString(), result.Message);
    }
}