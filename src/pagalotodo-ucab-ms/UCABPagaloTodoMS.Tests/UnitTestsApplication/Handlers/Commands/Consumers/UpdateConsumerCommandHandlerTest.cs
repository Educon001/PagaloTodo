using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Consumers;

public class UpdateConsumerCommandHandlerTest
{
    private readonly UpdateConsumerCommandHandler _handler;
    private readonly Mock<ILogger<UpdateConsumerCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdateConsumerCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdateConsumerCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdateConsumerCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdateConsumerCommandHandler_Ok_PasswordNull()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = ConsumerMapper.MapEntityToResponse(entity);
        expectedResponse.FullName = "New Name";
        var request = new ConsumerRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = "New",
            ConsumerId = entity.ConsumerId,
            Status = entity.Status,
            LastName = "Name",
            PasswordHash = null
        };
        _mockContext.Setup(m => m.Consumers.Find(entity.Id)).Returns(entity);
        var command = new UpdateConsumerCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<ConsumerResponse>(response);
        Assert.Equal(expectedResponse.FullName, response.FullName);
    }

    [Fact]
    public async void UpdateConsumerCommandHandler_Ok_PasswordNotNull()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = ConsumerMapper.MapEntityToResponse(entity);
        expectedResponse.FullName = "New Name";
        var request = new ConsumerRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = "New",
            ConsumerId = entity.ConsumerId,
            Status = entity.Status,
            LastName = "Name",
            PasswordHash = entity.PasswordHash
        };
        _mockContext.Setup(m => m.Consumers.Find(entity.Id)).Returns(entity);
        var command = new UpdateConsumerCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<ConsumerResponse>(response);
        Assert.Equal(expectedResponse.FullName, response.FullName);
    }
    
    [Fact]
    public async void UpdateConsumerCommandHandler_ValidationException()
    {
        var request = new ConsumerRequest()
        {
            Username = "HandlerTest"
        };
        var command = new UpdateConsumerCommand(request,new Guid());
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
    
    [Fact]
    public async void UpdateConsumerCommandHandle_ArgumentNullException()
    {
        var command = new UpdateConsumerCommand(null,new Guid());
        await Assert.ThrowsAsync<ArgumentNullException>(() =>_handler.Handle(command, default));
    }
    
    [Fact]
    public async void UpdateConsumerCommandHandler_HandleAsyncException()
    {
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
        var command = new UpdateConsumerCommand(request,entity.Id);
        var result = await Assert.ThrowsAsync<KeyNotFoundException>(()=>_handler.Handle(command, default));
        Assert.Contains(entity.Id.ToString(),result.Message);
    }
}