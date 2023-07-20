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
public class GetProviderByIdQueryHandlerTest
{
    private readonly GetProviderByIdQueryHandler _handler;
    private readonly Mock<ILogger<GetProviderByIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetProviderByIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetProviderByIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetProviderByIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetProviderByIdQueryHandle_Returns_ProviderResponse()
    {
        var entity = _mockContext.Object.Providers.First();
        var expectedResponse = ProviderMapper.MapEntityToResponse(entity);
        var query = new GetProviderByIdQuery(entity.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<ProviderResponse>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetProviderByIdQueryHandle_HandleAsyncException()
    {
        var entity = _mockContext.Object.Providers.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Providers).Throws(expectedException);
        var query = new GetProviderByIdQuery(entity.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetProviderByIdQueryHandle_ArgumentNullException()
    {
        var query = new GetProviderByIdQuery(Guid.Empty);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(query, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}