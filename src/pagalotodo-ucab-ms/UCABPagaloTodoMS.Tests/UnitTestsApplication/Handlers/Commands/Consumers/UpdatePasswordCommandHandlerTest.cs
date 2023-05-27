using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Consumers;

public class UpdatePasswordCommandHandlerTest
{
    private readonly UpdatePasswordCommandHandler _handler;
    private readonly Mock<ILogger<UpdatePasswordCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdatePasswordCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdatePasswordCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdatePasswordCommandHandler(_mockContext.Object,_loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdatePasswordCommandHandle_Ok()
    {
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "NewPassword!"
        };
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = new UpdatePasswordResponse(entity.Id, "Password updated successfully.");
        _mockContext.Setup(m => m.Consumers.Find(entity.Id)).Returns(entity);
        var command = new UpdatePasswordCommand(request, entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<UpdatePasswordResponse>(response);
        Assert.Equal(expectedResponse.ToString(),response.ToString());
    }
    
    [Fact]
    public async void UpdatePasswordCommandHandle_ValidationException()
    {
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "invalid password"
        };
        var command = new UpdatePasswordCommand(request, Guid.NewGuid());
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ValidationException>(result.InnerException);
    }
    
    [Fact]
    public async void UpdatePasswordCommandHandle_ArgumentNullException()
    {
        var command = new UpdatePasswordCommand(null, new Guid());
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
    
    [Fact]
    public async void UpdatePasswordCommandHandle_HandleAsyncException()
    {
        var request = new UpdatePasswordRequest()
        {
            PasswordHash = "NewPassword!"
        };
        var entity = _mockContext.Object.Consumers.First();
        var command = new UpdatePasswordCommand(request, entity.Id);
        var response = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command, default));
        Assert.IsType<KeyNotFoundException>(response.InnerException);
        Assert.Contains(entity.Id.ToString(), response.Message);
    }
}