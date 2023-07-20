using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries.Payments;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries.Payments;

[ExcludeFromCodeCoverage]
public class GetPaymentFieldsByServiceIdQueryHandlerTest
{
    private readonly GetPaymentFieldsByServiceIdQueryHandler _handler;
    private readonly Mock<ILogger<GetPaymentFieldsByServiceIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetPaymentFieldsByServiceIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetPaymentFieldsByServiceIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetPaymentFieldsByServiceIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetPaymentFieldsByServiceIdQueryHandle_Returns_List()
    {
        var service = _mockContext.Object.Services.First();
        var expectedResponse = service.PaymentFormat!.Select(PaymentFieldMapper.MapEntityToResponse).ToList();
        var query = new GetPaymentFieldsByServiceIdQuery(service.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<PaymentFieldResponse>>(response);
        Assert.Equal(expectedResponse!.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetPaymentFieldsByServiceIdQueryHandle_HandleAsyncException()
    {
        var service = _mockContext.Object.Services.Skip(1).First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.PaymentFields).Throws(expectedException);
        var query = new GetPaymentFieldsByServiceIdQuery(service.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetPaymentFieldsByServiceIdQueryHandle_ArgumentNullException()
    {
        var request = new GetPaymentFieldsByServiceIdQuery(Guid.Empty);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(request, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}