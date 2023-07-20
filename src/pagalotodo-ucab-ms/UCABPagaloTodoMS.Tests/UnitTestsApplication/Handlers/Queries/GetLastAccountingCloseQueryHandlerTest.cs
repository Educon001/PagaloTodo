using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

[ExcludeFromCodeCoverage]
public class GetLastAccountingCloseQueryHandlerTest
{
    private readonly GetLastAccountingCloseQueryHandler _handler;
    private readonly Mock<ILogger<GetLastAccountingCloseQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetLastAccountingCloseQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetLastAccountingCloseQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetLastAccountingCloseQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetLastAccountingCloseQueryHandle_Returns_DateTime()
    {
        var expectedResponse = _mockContext.Object.AccountingClosures.Last().ExecutedAt;
        var query = new GetLastAccountingCloseQuery();
        var response = await _handler.Handle(query,default);
        Assert.IsType<DateTime>(response);
        Assert.Equal(expectedResponse, response);
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetLastAccountingCloseQueryHandle_HandleAsyncException()
    {
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.AccountingClosures).Throws(expectedException);
        var query = new GetLastAccountingCloseQuery();
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
}