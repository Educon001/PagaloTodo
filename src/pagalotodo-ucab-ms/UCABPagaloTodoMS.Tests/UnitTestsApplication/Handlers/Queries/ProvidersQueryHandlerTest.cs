using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Handlers.Queries.Providers;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

[ExcludeFromCodeCoverage]
public class ProvidersQueryHandlerTest
{
    private readonly GetProvidersQueryHandler _handler;
    private readonly Mock<ILogger<GetProvidersQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public ProvidersQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetProvidersQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetProvidersQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void ProvidersQueryHandle_Returns_List()
    {
        var expectedResponse = _mockContext.Object.Providers.Select(p => ProviderMapper.MapEntityToResponse(p)).ToList();
        var query = new GetProvidersQuery();
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<ProviderResponse>>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void ProvidersQueryHandle_HandleAsyncException()
    {
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Providers).Throws(expectedException);
        var query = new GetProvidersQuery();
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void ProvidersQueryHandle_ArgumentNullException()
    {
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(null, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}