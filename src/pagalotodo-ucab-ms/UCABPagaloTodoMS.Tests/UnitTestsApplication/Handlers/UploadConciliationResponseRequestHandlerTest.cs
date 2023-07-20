using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Handlers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Services;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers;

[ExcludeFromCodeCoverage]
public class UploadConciliationResponseRequestHandlerTest
{
    private readonly UploadConciliationResponseRequestHandler _handler;
    private readonly Mock<ILogger<UploadConciliationResponseRequestHandler>> _loggerMock;
    private readonly Mock<IRabbitMqProducer> _mockProducer;

    public UploadConciliationResponseRequestHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UploadConciliationResponseRequestHandler>>();
        _mockProducer = new Mock<IRabbitMqProducer>();
        var mockResolver = new Mock<ProducerResolver>();
        mockResolver.Setup(r => r("Conciliation")).Returns(_mockProducer.Object);
        _handler = new UploadConciliationResponseRequestHandler(_loggerMock.Object, mockResolver.Object);
    }

    /// <summary>
    ///     Prueba de handler con respuesta true
    /// </summary>
    [Fact]
    public async void UploadConciliationResponseRequestHandle_Returns_True()
    {
        var data = new byte[]
        {
            1, 2, 3, 4
        };
        var request = new UploadConciliationResponseRequest(data);
        var response = await _handler.Handle(request, default);
        Assert.IsType<bool>(response);
        Assert.True(response);
    }


    /// <summary>
    ///     Prueba de handler con respuesta false
    /// </summary>
    [Fact]
    public async void UploadConciliationResponseRequestHandle_Returns_False()
    {
        var data = new byte[]
        {
            1, 2, 3, 4
        };
        _mockProducer.Setup(p => p.PublishMessage(It.Is<byte[]>(b => b.Length == 4)))
            .Throws(new Exception());
        var request = new UploadConciliationResponseRequest(data);
        var response = await _handler.Handle(request, default);
        Assert.IsType<bool>(response);
        Assert.False(response);
    }
}