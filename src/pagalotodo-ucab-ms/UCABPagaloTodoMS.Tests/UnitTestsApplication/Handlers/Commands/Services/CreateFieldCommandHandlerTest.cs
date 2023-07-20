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
public class CreateFieldCommandHandlerTest
{
    private readonly CreateFieldCommandHandler _handler;
    private readonly Mock<ILogger<CreateFieldCommandHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public CreateFieldCommandHandlerTest()
    {
        _loggerMock = new Mock<ILogger<CreateFieldCommandHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        _handler = new CreateFieldCommandHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    [Fact]
    public async void CreateFieldCommandHandler_Ok()
    {
        var expectedResponse = Guid.NewGuid();
        var entity = _mockContext.Object.Fields.First();
        var request = new FieldRequest()
        {
            Name = entity.Name,
            Format = entity.Format,
            Length = entity.Length,
            Service = entity.Service!.Id,
            AttrReference = entity.AttrReference
        };
        _mockContext.Setup(c => c.Services.Find(entity.Service.Id)).Returns(entity.Service);
        _mockContext.Setup(c => c.Fields.Add(It.IsAny<FieldEntity>()))
            .Callback((FieldEntity p) => p.Id = expectedResponse);
        var command = new CreateFieldCommand(request);
        var response = await _handler.Handle(command, default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async void CreateFieldCommandHandle_ArgumentNullException()
    {
        var command = new CreateFieldCommand(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(command, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }

    [Fact]
    public async void CreateFieldCommandHandler_HandleAsyncException()
    {
        var entity = _mockContext.Object.Fields.First();
        var request = new FieldRequest()
        {
            Name = entity.Name,
            Format = entity.Format,
            Length = entity.Length,
            Service = entity.Service!.Id,
            AttrReference = entity.AttrReference
        };
        var command = new CreateFieldCommand(request);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Contains(entity.Service!.Id.ToString(), result.Message);
    }
}