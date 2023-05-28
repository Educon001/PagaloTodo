using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Services;

public class UpdateFieldCommandHandlerTest
{
    private readonly UpdateFieldCommandHandler _handler;
    private readonly Mock<ILogger<UpdateFieldCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdateFieldCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdateFieldCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdateFieldCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdateFieldCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Fields.First();
        var expectedResponse = FieldMapper.MapEntityToResponse(entity);
        expectedResponse.Name = "New Name";
        var request = new FieldRequest()
        {
            Name = "New Name",
            Format = entity.Format,
            Length = entity.Length,
            AttrReference = entity.AttrReference
        };
        _mockContext.Setup(m => m.Fields.Find(entity.Id)).Returns(entity);
        var command = new UpdateFieldCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<FieldResponse>(response);
        Assert.Equal(expectedResponse.Name, response.Name);
    }

    [Fact]
    public async void UpdateFieldCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Fields.First();
        var request = new FieldRequest()
        {
            Name = "New Name",
            Format = entity.Format,
            Length = entity.Length,
            AttrReference = entity.AttrReference
        };
        var command = new UpdateFieldCommand(request,entity.Id);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command, default));
        Assert.IsType<NotImplementedException>(result.InnerException);
    }
}