using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers.Queries.Payments;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetPaymentsByServiceIdQueryHandlerTest
{
    private readonly GetPaymentsByServiceIdQueryHandler _handler;
    private readonly Mock<ILogger<GetPaymentsByServiceIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetPaymentsByServiceIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetPaymentsByServiceIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetPaymentsByServiceIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentsByServiceIdQueryHandle_Returns_List()
    {
        var service = _mockContext.Object.Services.First();
        var expectedResponse = service.Payments!.Select(PaymentMapper.MapEntityToResponse).ToList();
        var query = new GetPaymentsByServiceIdQuery(service.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<PaymentResponse>>(response);
        Assert.Equal(expectedResponse!.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetPaymentsByServiceIdQueryHandle_HandleAsyncException()
    {
        var service = _mockContext.Object.Services.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Payments).Throws(expectedException);
        var query = new GetPaymentsByServiceIdQuery(service.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetPaymentsByServiceIdQueryHandle_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(()=>_handler.Handle(null, default));
    }
}