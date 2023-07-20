using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Providers;

[ExcludeFromCodeCoverage]
public class DeleteProviderCommandHandlerTest
{
    private readonly DeleteProviderCommandHandler _handler;
    private readonly Mock<ILogger<DeleteProviderCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public DeleteProviderCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<DeleteProviderCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new DeleteProviderCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void DeleteProviderCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Providers.First();
        var command = new DeleteProviderCommand(entity.Id);
        _mockContext.Setup(m => m.Providers.Find(entity.Id)).Returns(entity);
        var response = await _handler.Handle(command,default);
        Assert.IsType<Guid>(response);
        Assert.Equal(entity.Id,response);
    }
    
    [Fact]
    public async void DeleteProviderCommandHandler_HandleAsyncException()
    {
        var id = Guid.NewGuid();
        var command = new DeleteProviderCommand(id);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command,default));
        Assert.IsType<KeyNotFoundException>(result.InnerException);
        Assert.Contains(id.ToString(),result.Message);
    }
    
    [Fact]
    public async void DeleteProviderCommandHandler_ArgumentNullException()
    {
        var command = new DeleteProviderCommand(Guid.Empty);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command,default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}