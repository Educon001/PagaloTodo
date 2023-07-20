using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands.Services;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Services;

[ExcludeFromCodeCoverage]
public class CreateServiceCommandHandlerTest
{
    private readonly CreateServiceCommandHandler _handler;
    private readonly Mock<ILogger<CreateServiceCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreateServiceCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreateServiceCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreateServiceCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreateServiceCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var entity = _mockContext.Object.Services.First();
        var request = new ServiceRequest()
        {
            Name = entity.Name,
            Description = entity.Description,
            ServiceStatus = entity.ServiceStatus,
            ServiceType = entity.ServiceType,
            Provider = entity.Provider!.Id
        };
        _mockContext.Setup(c => c.Providers.Find(entity.Provider.Id)).Returns(entity.Provider);
        _mockContext.Setup(c => c.Services.Add(It.IsAny<ServiceEntity>()))
            .Callback((ServiceEntity p) => p.Id = expectedResponse);
        var command = new CreateServiceCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void CreateServiceCommandHandle_ArgumentNullException()
    {
        var command = new CreateServiceCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }

    [Fact]
    public async void CreateServiceCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Services.First();
        var request = new ServiceRequest()
        {
            Name = entity.Name,
            Description = entity.Description,
            ServiceStatus = entity.ServiceStatus,
            ServiceType = entity.ServiceType,
            Provider = entity.Provider!.Id
        };
        var command = new CreateServiceCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Contains(entity.Provider!.Id.ToString(), result.Message);
    }
}