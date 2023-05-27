using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries.Payments;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries.Payments;

public class GetPaymentsByConsumerIdQueryHandlerTest
{
    private readonly GetPaymentsByConsumerIdQueryHandler _handler;
    private readonly Mock<ILogger<GetPaymentsByConsumerIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetPaymentsByConsumerIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetPaymentsByConsumerIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetPaymentsByConsumerIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentsByConsumerIdQueryHandle_Returns_List()
    {
        var consumer = _mockContext.Object.Consumers.First();
        var expectedResponse =
            consumer.Payments!.Select(p => PaymentMapper.MapEntityToResponse(p, false, true)).ToList();
        var query = new GetPaymentsByConsumerIdQuery(consumer.Id, null, null);
        var response = await _handler.Handle(query, default);
        Assert.IsType<List<PaymentResponse>>(response);
        Assert.Equal(expectedResponse!.ToString(), response.ToString());
    }


    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetPaymentsByConsumerIdQueryHandle_HandleAsyncException()
    {
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Payments).Throws(expectedException);
        var query = new GetPaymentsByConsumerIdQuery(Guid.NewGuid(), null, null);
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _handler.Handle(query, default));
        Assert.Equal(expectedException.Message, result.Message);
    }

    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetPaymentsByConsumerIdQueryHandle_ArgumentNullException()
    {
        var request = new GetPaymentsByConsumerIdQuery(Guid.Empty, null, null);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(request, default));
    }
}