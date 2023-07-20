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
public class GetServicesByProviderIdQueryHandlerTest
{
    private readonly GetServicesByProviderIdQueryHandler _handler;
    private readonly Mock<ILogger<GetServicesByProviderIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetServicesByProviderIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetServicesByProviderIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetServicesByProviderIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetServicesByProviderIdQueryHandle_Returns_List()
    {
        var provider = _mockContext.Object.Providers.First();
        var expectedResponse = provider.Services!.Select(s=>ServiceMapper.MapEntityToResponse(s,false)).ToList();
        var query = new GetServicesByProviderIdQuery(provider.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<ServiceResponse>>(response);
        Assert.Equal(expectedResponse!.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetServicesByProviderIdQueryHandle_HandleAsyncException()
    {
        var provider = _mockContext.Object.Providers.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Services).Throws(expectedException);
        var query = new GetServicesByProviderIdQuery(provider.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetServicesByProviderIdQueryHandle_ArgumentNullException()
    {
        var query = new GetServicesByProviderIdQuery(Guid.Empty);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(query, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}