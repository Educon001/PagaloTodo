using System.Net;

namespace UCABPagaloTodoMS.Core.Services;

public interface IEmailSender
{
    Task<HttpStatusCode> SendEmailAsync(string email, string url);
}