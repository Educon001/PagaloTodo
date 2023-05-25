using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetServiceByIdQueryHandlerTest
{
    private readonly GetServiceByIdQueryHandler _handler;
    private readonly Mock<ILogger<GetServiceByIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetServiceByIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetServiceByIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetServiceByIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetServiceByIdQueryHandle_Returns_ServiceResponse()
    {
        var entity = _mockContext.Object.Services.First();
        var expectedResponse = ServiceMapper.MapEntityToResponse(entity,false);
        var query = new GetServiceByIdQuery(entity.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<ServiceResponse>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetServiceByIdQueryHandle_HandleAsyncException()
    {
        var entity = _mockContext.Object.Services.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Services).Throws(expectedException);
        var query = new GetServiceByIdQuery(entity.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetServiceByIdQueryHandle_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(()=>_handler.Handle(null, default));
    }
}