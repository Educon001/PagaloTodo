using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Services;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers;

[ExcludeFromCodeCoverage]
public class UploadDebtorsRequestHandlerTest
{
    private readonly UploadDebtorsRequestHandler _handler;
    private readonly Mock<ILogger<UploadDebtorsRequestHandler>> _loggerMock;
    private readonly Mock<IRabbitMqProducer> _mockProducer;

    public UploadDebtorsRequestHandlerTest()
    {
        _loggerMock = new Mock<ILogger<UploadDebtorsRequestHandler>>();
        _mockProducer = new Mock<IRabbitMqProducer>();
        var mockResolver = new Mock<ProducerResolver>();
        mockResolver.Setup(r => r("Confirmation")).Returns(_mockProducer.Object);
        _handler = new UploadDebtorsRequestHandler(_loggerMock.Object, mockResolver.Object);
    }

    /// <summary>
    ///     Prueba de handler con respuesta true
    /// </summary>
    [Fact]
    public async void UploadDebtorsRequestHandle_Returns_True()
    {
        var data = new byte[]
        {
            1, 2, 3, 4
        };
        var serviceId = Guid.NewGuid();
        var request = new UploadDebtorsRequest(data, serviceId);
        var response = await _handler.Handle(request, default);
        Assert.IsType<bool>(response);
        Assert.True(response);
    }


    /// <summary>
    ///     Prueba de handler con respuesta false
    /// </summary>
    [Fact]
    public async void UploadDebtorsRequestHandle_Returns_False()
    {
        var data = new byte[]
        {
            1, 2, 3, 4
        };
        var serviceId = Guid.NewGuid();
        _mockProducer.Setup(p => p.PublishMessage(It.Is<byte[]>(b => b.Length == 20)))
            .Throws(new Exception());
        var request = new UploadDebtorsRequest(data, serviceId);
        var response = await _handler.Handle(request, default);
        Assert.IsType<bool>(response);
        Assert.False(response);
    }
}