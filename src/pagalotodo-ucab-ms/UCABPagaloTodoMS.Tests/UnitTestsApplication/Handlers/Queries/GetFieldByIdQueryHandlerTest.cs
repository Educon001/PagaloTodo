using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Handlers.Queries.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

[ExcludeFromCodeCoverage]
public class GetFieldByIdQueryHandlerTest
{
    private readonly GetFieldByIdQueryHandler _handler;
    private readonly Mock<ILogger<GetFieldByIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetFieldByIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetFieldByIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetFieldByIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetFieldByIdQueryHandle_Returns_FieldResponse()
    {
        var entity = _mockContext.Object.Fields.First();
        var expectedResponse = FieldMapper.MapEntityToResponse(entity);
        var query = new GetFieldByIdQuery(entity.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<FieldResponse>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetFieldByIdQueryHandle_HandleAsyncException()
    {
        var entity = _mockContext.Object.Fields.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Fields).Throws(expectedException);
        var query = new GetFieldByIdQuery(entity.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetFieldByIdQueryHandle_ArgumentNullException()
    {
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(null, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}