using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Consumers;

[ExcludeFromCodeCoverage]
public class DeleteConsumerCommandHandlerTest
{
    private readonly DeleteConsumerCommandHandler _handler;
    private readonly Mock<ILogger<DeleteConsumerCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public DeleteConsumerCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<DeleteConsumerCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new DeleteConsumerCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void DeleteConsumerCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Consumers.First();
        var command = new DeleteConsumerCommand(entity.Id);
        _mockContext.Setup(m => m.Consumers.Find(entity.Id)).Returns(entity);
        var response = await _handler.Handle(command,default);
        Assert.IsType<Guid>(response);
        Assert.Equal(entity.Id,response);
    }
    
    [Fact]
    public async void DeleteConsumerCommandHandler_HandleAsyncException()
    {
        var id = Guid.NewGuid();
        var command = new DeleteConsumerCommand(id);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<CustomException>(result.InnerException);
        Assert.Contains($"Object with key {id} not found", result.InnerException.Message);
    }
}