using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Services;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers;

[ExcludeFromCodeCoverage]
public class AccountingCloseRequestHandlerTest
{
    private readonly AccountingCloseRequestHandler _handler;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;
    private readonly Mock<ILogger<AccountingCloseRequestHandler>> _loggerMock;
    private readonly Mock<IEmailSender> _mockSender;
    private readonly Mock<IDbContextTransactionProxy> _mockTransaction;

    public AccountingCloseRequestHandlerTest()
    {
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        _loggerMock = new Mock<ILogger<AccountingCloseRequestHandler>>();
        _mockSender = new Mock<IEmailSender>();
        _mockTransaction = new Mock<IDbContextTransactionProxy>();
        var mockResolver = new Mock<SenderResolver>();
        mockResolver.Setup(r => r("Conciliation")).Returns(_mockSender.Object);
        _handler = new AccountingCloseRequestHandler(_mockContext.Object, mockResolver.Object, _loggerMock.Object);
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
        _mockContext.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
    }

    /// <summary>
    ///     Prueba de handler con respuesta ok
    /// </summary>
    [Fact]
    public async void AccountingCloseRequestHandle_Returns_String()
    {
        var expectedResponse = "Se emitieron 2 archivos a 2 prestadores. Errores: 0";
        var providers = _mockContext.Object.Providers.Select(ProviderMapper.MapEntityToResponse).ToList();
        var request = new AccountingCloseRequest(providers);
        _mockSender.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<List<CsvResponse>>()))
            .ReturnsAsync(HttpStatusCode.Accepted);
        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.IsType<string>(result);
        Assert.Equal(expectedResponse, result);
    }


    /// <summary>
    ///     Prueba de handler con respuesta 
    /// </summary>
    [Fact]
    public async void AccountingCloseRequestHandle_ProvidersNull_Exception()
    {
        var request = new AccountingCloseRequest(null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }

    [Fact]
    public async void AccountingCloseRequestHandle_DbContext_Exception()
    {
        var expectedException = new Exception("Test Exception");
        var providers = _mockContext.Object.Providers.Select(ProviderMapper.MapEntityToResponse).ToList();
        var request = new AccountingCloseRequest(providers);
        _mockSender.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<List<CsvResponse>>()))
            .ReturnsAsync(HttpStatusCode.Accepted);
        _mockContext.Setup(c => c.AccountingClosures.Add(It.IsAny<AccountingCloseEntity>())).Throws(expectedException);
        var result = await Assert.ThrowsAsync<CustomException>(()=>_handler.Handle(request, CancellationToken.None));
        Assert.Equal(expectedException, result.InnerException);
    }

    [Fact]
    public async void AccountingCloseRequestHandle_EmailSender_BadRequest()
    {
        var expectedResponse = "Se emitieron 2 archivos a 2 prestadores. Errores: 2";
        var providers = _mockContext.Object.Providers.Select(ProviderMapper.MapEntityToResponse).ToList();
        var request = new AccountingCloseRequest(providers);
        _mockSender.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<List<CsvResponse>>()))
            .ReturnsAsync(HttpStatusCode.BadRequest);
        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.IsType<string>(result);
        Assert.Equal(expectedResponse, result);
    }
}