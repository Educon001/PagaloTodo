using System.Diagnostics.CodeAnalysis;
using System.Net;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Models;
using UCABPagaloTodoMS.Infrastructure.Services;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTestsInfrastructure.Services;
[ExcludeFromCodeCoverage]
public class ConciliationEmailSenderTest
{
    private readonly ConciliationEmailSender _sender;
    private readonly Mock<ISendGridClient> _mockClient;

    public ConciliationEmailSenderTest()
    {
        _mockClient = new Mock<ISendGridClient>();
        _sender = new ConciliationEmailSender(_mockClient.Object, "test@email.com", "Test");
    }

    [Fact]
    public async void SendEmailAsync_Test()
    {
        var email = "prueba@email.com";
        var body = new List<CsvResponse>()
        {
            new("file1.csv", new byte[] {1, 2, 3}),
            new("file2.csv", new byte[] {1, 2, 3, 4})
        };
        var expectedResponse =
            new Response(HttpStatusCode.Accepted, null,null);
        _mockClient.Setup(m => m.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _sender.SendEmailAsync(email,body);
        Assert.IsType<HttpStatusCode>(response);
        Assert.Equal(expectedResponse.StatusCode,response);
    }
}