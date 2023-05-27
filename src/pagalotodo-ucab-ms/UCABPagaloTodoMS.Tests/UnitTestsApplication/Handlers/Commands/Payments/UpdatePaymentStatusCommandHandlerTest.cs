using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Handlers.Commands.Payments;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Payments;

public class UpdatePaymentStatusCommandHandlerTest
{
    private readonly UpdatePaymentStatusCommandHandler _handler;
    private readonly Mock<ILogger<UpdatePaymentStatusCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdatePaymentStatusCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdatePaymentStatusCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdatePaymentStatusCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdatePaymentStatusCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Payments.First();
        var request = PaymentStatusEnum.Pendiente;
        var expectedResponse = entity.Id + " PaymentStatus updated successfully";
        _mockContext.Setup(c => c.Payments.Find(entity.Id)).Returns(entity);
        var command = new UpdatePaymentStatusCommand(entity.Id, request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<string>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void UpdatePaymentStatusCommandHandle_ArgumentNullException()
    {
        var command = new UpdatePaymentStatusCommand(Guid.Empty, null);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async void UpdatePaymentStatusCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Payments.First();
        var request = PaymentStatusEnum.Pendiente;
        var command = new UpdatePaymentStatusCommand(entity.Id, request);
        var result = await Assert.ThrowsAnyAsync<KeyNotFoundException>(()=>_handler.Handle(command,default));
        Assert.Contains(entity.Id.ToString(), result.Message);
    }
}