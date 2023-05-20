using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetServicesQueryHandlerTest
{
    private readonly GetServicesQueryHandler _handler;
    private readonly Mock<ILogger<GetServicesQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetServicesQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetServicesQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetServicesQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetServicesQueryHandle_Returns_List()
    {
        var expectedResponse = _mockContext.Object.Services.Select(s => ServiceMapper.MapEntityToResponse(s)).ToList();
        var query = new GetServicesQuery();
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<ServiceResponse>>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetServicesQueryHandle_HandleAsyncException()
    {
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Services).Throws(expectedException);
        var query = new GetServicesQuery();
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void ProvidersQueryHandle_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(()=>_handler.Handle(null, default));
    }
}