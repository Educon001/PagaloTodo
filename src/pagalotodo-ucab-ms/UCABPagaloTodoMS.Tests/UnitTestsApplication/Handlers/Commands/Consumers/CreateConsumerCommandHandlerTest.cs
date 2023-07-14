using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Consumers;

public class CreateConsumerCommandHandlerTest
{
    private readonly CreateConsumerCommandHandler _handler;
    private readonly Mock<ILogger<CreateConsumerCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreateConsumerCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreateConsumerCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreateConsumerCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreateConsumerCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var entity = _mockContext.Object.Consumers.First();
        var request = new ConsumerRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = entity.Name,
            ConsumerId = entity.ConsumerId,
            Status = entity.Status,
            LastName = entity.LastName,
            PasswordHash = entity.PasswordHash
        };
        _mockContext.Setup(c => c.Consumers.Add(It.IsAny<ConsumerEntity>()))
            .Callback((ConsumerEntity p) => p.Id = expectedResponse );
        var command = new CreateConsumerCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void CreateConsumerCommandHandler_ValidationException()
    {
        var request = new ConsumerRequest()
        {
            Username = "HandlerTest"
        };
        var command = new CreateConsumerCommand(request);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ValidationException>(result.InnerException);
    }
    
    [Fact]
    public async void CreateConsumerCommandHandle_ArgumentNullException()
    {
        var command = new CreateConsumerCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() =>_handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
    
    [Fact]
    public async void CreateConsumerCommandHandler_HandleAsyncException()
    {
        var expectedException = new Exception("Test exception");
        var entity = _mockContext.Object.Consumers.First();
        var request = new ConsumerRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = entity.Name,
            ConsumerId = entity.ConsumerId,
            Status = entity.Status,
            LastName = entity.LastName,
            PasswordHash = entity.PasswordHash
        };
        _mockContext.Setup(c => c.Consumers.Add(It.IsAny<ConsumerEntity>()))
            .Throws(expectedException);
        var command = new CreateConsumerCommand(request);
        var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.Equal(expectedException.Message, exception.InnerException.Message);
    }
}