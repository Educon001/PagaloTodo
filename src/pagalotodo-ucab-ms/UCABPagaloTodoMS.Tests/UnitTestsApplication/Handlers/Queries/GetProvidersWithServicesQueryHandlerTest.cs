using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Handlers.Queries.Providers;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

[ExcludeFromCodeCoverage]
public class GetProvidersWithServicesQueryHandlerTest
{
    private readonly GetProvidersWithServicesQueryHandler _handler;
    private readonly Mock<ILogger<GetProvidersWithServicesQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetProvidersWithServicesQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetProvidersWithServicesQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetProvidersWithServicesQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void ProvidersWithServicesQueryHandle_Returns_List()
    {
        var expectedResponse = _mockContext.Object.Providers.Select(p=>ProviderMapper.MapEntityToResponse(p)).ToList();
        var query = new GetProvidersWithServicesQuery();
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<ProviderResponse>>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void ProvidersWithServicesQueryHandle_HandleAsyncException()
    {
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Providers).Throws(expectedException);
        var query = new GetProvidersWithServicesQuery();
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void ProvidersWithServicesQueryHandle_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(()=>_handler.Handle(null, default));
    }
}