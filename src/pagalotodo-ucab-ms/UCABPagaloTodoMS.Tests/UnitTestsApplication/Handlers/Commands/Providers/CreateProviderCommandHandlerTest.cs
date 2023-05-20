using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Handlers.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Commands.Providers;

public class CreateProviderCommandHandlerTest
{
    private readonly CreateProviderCommandHandler _handler;
    private readonly Mock<ILogger<CreateProviderCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreateProviderCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreateProviderCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreateProviderCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreateProviderCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
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
        _mockContext.Setup(c => c.Providers.Add(It.IsAny<ProviderEntity>()))
            .Callback((ProviderEntity p) => p.Id = expectedResponse );
        var command = new CreateProviderCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void CreateProviderCommandHandler_ValidationException()
    {
        var request = new ProviderRequest()
        {
            Username = "HandlerTest"
        };
        var command = new CreateProviderCommand(request);
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
    
    [Fact]
    public async void CreateProviderCommandHandle_ArgumentNullException()
    {
        var command = new CreateProviderCommand(null);
        await Assert.ThrowsAsync<ArgumentNullException>(() =>_handler.Handle(command, default));
    }
    
    [Fact]
    public async void CreateProviderCommandHandler_HandleAsyncException()
    {
        var expectedException = new Exception("Test exception");
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
        _mockContext.Setup(c => c.Providers.Add(It.IsAny<ProviderEntity>()))
            .Throws(expectedException);
        var command = new CreateProviderCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(command, default));
        Assert.Equal(expectedException.Message, result.Message);
    }
}