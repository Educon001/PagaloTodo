using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Providers;

public class UpdateProviderCommandHandlerTest
{
    private readonly UpdateProviderCommandHandler _handler;
    private readonly Mock<ILogger<UpdateProviderCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public UpdateProviderCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UpdateProviderCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new UpdateProviderCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void UpdateProviderCommandHandler_Ok_PasswordNull()
    {
        var entity = _mockContext.Object.Providers.First();
        var expectedResponse = ProviderMapper.MapEntityToResponse(entity);
        expectedResponse.Name = "New";
        var request = new ProviderRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = "New",
            Rif = entity.Rif,
            Status = entity.Status,
            AccountNumber = entity.AccountNumber,
            LastName = "Name",
            PasswordHash = null
        };
        _mockContext.Setup(m => m.Providers.Find(entity.Id)).Returns(entity);
        var command = new UpdateProviderCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<ProviderResponse>(response);
        Assert.Equal(expectedResponse.Name, response.Name);
    }

    [Fact]
    public async void UpdateProviderCommandHandler_Ok_PasswordNotNull()
    {
        var entity = _mockContext.Object.Providers.First();
        var expectedResponse = ProviderMapper.MapEntityToResponse(entity);
        expectedResponse.Name = "New";
        var request = new ProviderRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = "New",
            Rif = entity.Rif,
            Status = entity.Status,
            AccountNumber = entity.AccountNumber,
            LastName = "Name",
            PasswordHash = entity.PasswordHash
        };
        _mockContext.Setup(m => m.Providers.Find(entity.Id)).Returns(entity);
        var command = new UpdateProviderCommand(request,entity.Id);
        var response = await _handler.Handle(command, default);
        Assert.IsType<ProviderResponse>(response);
        Assert.Equal(expectedResponse.Name, response.Name);
    }
    
    [Fact]
    public async void UpdateProviderCommandHandler_ValidationException()
    {
        var request = new ProviderRequest()
        {
            Username = "HandlerTest"
        };
        var command = new UpdateProviderCommand(request,new Guid());
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ValidationException>(result.InnerException);
    }
    
    [Fact]
    public async void UpdateProviderCommandHandle_ArgumentNullException()
    {
        var command = new UpdateProviderCommand(null,new Guid());
        var result = await Assert.ThrowsAsync<CustomException>(() =>_handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
    
    [Fact]
    public async void UpdateProviderCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Providers.First();
        var request = new ProviderRequest()
        {
            Username = entity.Username,
            Email = entity.Email,
            Name = entity.Name,
            Rif = entity.Rif,
            Status = entity.Status,
            AccountNumber = entity.AccountNumber,
            LastName = entity.LastName,
            PasswordHash = entity.PasswordHash
        };
        var command = new UpdateProviderCommand(request,entity.Id);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(command, default));
        Assert.IsType<KeyNotFoundException>(result.InnerException);
        Assert.Contains(entity.Id.ToString(),result.Message);
    }
}