using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Handlers.Queries.Debtors;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Debtors;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers.Queries;

public class GetDebtorsByServiceIdQueryHandlerTest
{
    private readonly GetDebtorsByServiceIdQueryHandler _handler;
    private readonly Mock<ILogger<GetDebtorsByServiceIdQueryHandler>> _loggerMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public GetDebtorsByServiceIdQueryHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GetDebtorsByServiceIdQueryHandler>>();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _handler = new GetDebtorsByServiceIdQueryHandler(_mockContext.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }
    
    /// <summary>
    ///     Prueba de handler con respuesta Ok
    /// </summary>
    [Fact]
    public async void GetDebtorsByServiceIdQueryHandle_Returns_List()
    {
        var service = _mockContext.Object.Services.Skip(1).First();
        var expectedResponse = service.ConfirmationList!.Select(DebtorsMapper.MapEntityToResponse).ToList();
        var query = new GetDebtorsByServiceIdQuery(service.Id);
        var response = await _handler.Handle(query,default);
        Assert.IsType<List<DebtorsResponse>>(response);
        Assert.Equal(expectedResponse!.ToString(), response.ToString());
    }
    
    
    /// <summary>
    ///     Prueba de handler con excepción en HandleAsync
    /// </summary>
    [Fact]
    public async void GetDebtorsByServiceIdQueryHandle_HandleAsyncException()
    {
        var service = _mockContext.Object.Services.First();
        var expectedException = new Exception("Test Exception");
        _mockContext.Setup(c => c.Debtors).Throws(expectedException);
        var query = new GetDebtorsByServiceIdQuery(service.Id);
        var result = await Assert.ThrowsAnyAsync<Exception>(()=>_handler.Handle(query, default));
        Assert.Equal(expectedException.Message,result.Message);
    }
    
    /// <summary>
    ///     Prueba de handler con request nulo
    /// </summary>
    [Fact]
    public async void GetDebtorsByServiceIdQueryHandle_ArgumentNullException()
    {
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(null, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}