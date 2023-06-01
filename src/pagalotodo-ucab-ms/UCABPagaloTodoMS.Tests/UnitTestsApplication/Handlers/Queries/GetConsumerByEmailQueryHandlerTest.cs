using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries.Consumers;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Consumers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetConsumerByEmailQueryHandlerTest
{
    private readonly GetConsumerByEmailQueryHandler _handler;
    private readonly Mock<ILogger<GetConsumerByEmailQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetConsumerByEmailQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetConsumerByEmailQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetConsumerByEmailQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetConsumerByEmailQueryHandle_Returns_Guid()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedResponse = entity.Id;
        var query = new GetConsumerByEmailQuery(entity.Email);
        var response = await _handler.Handle(query,default);
        Assert.IsType<Guid>(response);
        Assert.Equal(expectedResponse.ToString(), response.ToString());
    }


    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetConsumerByEmailQueryHandle_HandleAsyncException()
    {
        var entity = _mockContext.Object.Consumers.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Consumers).Throws(expectedException);
        var query = new GetConsumerByEmailQuery(entity.Email);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }

    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetConsumerByEmailQueryHandle_ArgumentNullException()
    {
        var query = new GetConsumerByEmailQuery(null);
        var ex = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(query, default));
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
}