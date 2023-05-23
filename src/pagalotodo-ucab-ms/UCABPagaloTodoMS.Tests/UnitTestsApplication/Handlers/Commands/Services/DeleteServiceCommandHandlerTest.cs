using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Handlers.Commands.Services;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Services;

public class DeleteServiceCommandHandlerTest
{
    private readonly DeleteServiceCommandHandler _handler;
    private readonly Mock<ILogger<DeleteServiceCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public DeleteServiceCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<DeleteServiceCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new DeleteServiceCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void DeleteServiceCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Services.First();
        var command = new DeleteServiceCommand(entity.Id);
        _mockContext.Setup(m => m.Services.Find(entity.Id)).Returns(entity);
        var response = await _handler.Handle(command,default);
        Assert.IsType<Guid>(response);
        Assert.Equal(entity.Id,response);
    }
    
    [Fact]
    public async void DeleteServiceCommandHandler_HandleAsyncException()
    {
        var id = Guid.NewGuid();
        var command = new DeleteServiceCommand(id);
        var result = await Assert.ThrowsAsync<Exception>(()=>_handler.Handle(command,default));
        Assert.Contains(id.ToString(),result.Message);
    }
}