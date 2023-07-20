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
public class CreatePaymentFieldCommandHandlerTest
{
    private readonly CreatePaymentFieldCommandHandler _handler;
    private readonly Mock<ILogger<CreatePaymentFieldCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreatePaymentFieldCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreatePaymentFieldCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreatePaymentFieldCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }
    
    [Fact]
    public async void CreatePaymentFieldCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var service = _mockContext.Object.Services.First();
        var request = new PaymentFieldRequest()
        {
            Name = "Test Field",
            Format = "",
            Service = service.Id
        };
        _mockContext.Setup(c => c.Services.Find(request.Service)).Returns(service);
        _mockContext.Setup(c => c.PaymentFields.Add(It.IsAny<PaymentFieldEntity>()))
            .Callback((PaymentFieldEntity e) => e.Id = expectedResponse);
        var command = new CreatePaymentFieldCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }
    
    [Fact]
    public async void CreatePaymentFieldCommandHandler_ArgumentNullException()
    {
        var command = new CreatePaymentFieldCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
    
    [Fact]
    public async void CreatePaymentFieldCommandHandler_HandleAsyncException()
    {
        var service = _mockContext.Object.Services.First();
        var request = new PaymentFieldRequest()
        {
            Name = "Test Field",
            Format = "",
            Service = service.Id
        };
        var command = new CreatePaymentFieldCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Contains(service.Id.ToString(), result.Message);
    }
}