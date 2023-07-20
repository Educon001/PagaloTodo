using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Handlers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Services;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsApplication.Handlers;

[ExcludeFromCodeCoverage]
public class ForgotPasswordRequestHandlerTest
{
    private readonly ForgotPasswordRequestHandler _handler;
    private readonly Mock<ILogger<ForgotPasswordRequestHandler>> _loggerMock;
    private readonly Mock<IEmailSender> _senderMock;

    public ForgotPasswordRequestHandlerTest()
    {
        _loggerMock = new Mock<ILogger<ForgotPasswordRequestHandler>>();
        _senderMock = new Mock<IEmailSender>();
        _handler = new ForgotPasswordRequestHandler((key)=>_senderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async void Handle_Returns_SuccessfulResponse()
    {
        var request = new ForgotPasswordRequest("prueba@prueba.com", "https://url.com");
        var expectedResponse = new ForgotPasswordResponse(ForgotPasswordResponse.Successful);
        _senderMock.Setup(m => m.SendEmailAsync(request.UserEmail, request.Url)).ReturnsAsync(HttpStatusCode.Accepted);
        var result = await _handler.Handle(request, default);
        Assert.IsType<ForgotPasswordResponse>(result);
        Assert.Equal(expectedResponse.ToString(), result.ToString());
    }

    [Fact]
    public async void Handle_Returns_UnsuccessfulResponse()
    {
        var request = new ForgotPasswordRequest("prueba@prueba.com", "https://url.com");
        var expectedResponse = new ForgotPasswordResponse(ForgotPasswordResponse.Unsuccessful);
        _senderMock.Setup(m => m.SendEmailAsync(request.UserEmail, request.Url))
            .ReturnsAsync(HttpStatusCode.BadRequest);
        var result = await _handler.Handle(request, default);
        Assert.IsType<ForgotPasswordResponse>(result);
        Assert.Equal(expectedResponse.ToString(), result.ToString());
    }

    [Fact]
    public async void Handle_ArgumentNullException()
    {
        var request = new ForgotPasswordRequest(null, null);
        var result = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, default));
        Assert.IsType<ArgumentNullException>(result.InnerException);
    }
}