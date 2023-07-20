using System.Diagnostics.CodeAnalysis;
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

[ExcludeFromCodeCoverage]
public class UpdateServiceCommandHandlerTest
{
    private readonly UpdateServiceCommandHandler _handler;
    private readonly Mock<ILogger<UpdateServiceCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdateServiceCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdateServiceCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdateServiceCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdateServiceCommandHandler_Ok()
    {
        var entity = _mockContext.Object.Services.First();
        var expectedResponse = ServiceMapper.MapEntityToResponse(entity,false);
        expectedResponse.Name = "New Name";
        var request = new ServiceRequest()
        {
            Name = "New Name",
            Description = entity.Description,
            ServiceStatus = entity.ServiceStatus,
            ServiceType = entity.ServiceType
        };
        _mockContext.Setup(m => m.Services.Find(entity.Id)).Returns(entity);
        var command = new UpdateServiceCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<ServiceResponse>(response);
        Assert.Equal(expectedResponse.Name, response.Name);
    }

    [Fact]
    public async void UpdateServiceCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Services.First();
        var request = new ServiceRequest()
        {
            Name = "New Name",
            Description = entity.Description,
            ServiceStatus = entity.ServiceStatus,
            ServiceType = entity.ServiceType
        };
        var command = new UpdateServiceCommand(request,entity.Id);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command, default));
        Assert.IsType<NotImplementedException>(result.InnerException);
    }
}