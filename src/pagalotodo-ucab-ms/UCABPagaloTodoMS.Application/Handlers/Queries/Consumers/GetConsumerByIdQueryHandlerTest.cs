using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries.Consumers;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetConsumerByIdQueryHandlerTest
{
    private readonly GetConsumerByIdQueryHandler _handler;
    private readonly Mock<ILogger<GetConsumerByIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetConsumerByIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetConsumerByIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetConsumerByIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        //DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetConsumerByIdQueryHandle_Returns_ConsumerResponse()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = ConsumerMapper.MapEntityToResponse(entity);
        var query = new GetConsumerByIdQuery(entity.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<ConsumerResponse>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }


    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetConsumerByIdQueryHandle_HandleAsyncException()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Consumers).Throws(expectedException);
        var query = new GetConsumerByIdQuery(entity.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }

    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetConsumerByIdQueryHandle_ArgumentNullException()
    {
        var query = new GetConsumerByIdQuery(Guid.Empty);
        await Assert.ThrowsAsync<ArgumentNullException>(()=>_handler.Handle(query, default));
    }
}