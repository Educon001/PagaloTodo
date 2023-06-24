using System.Net;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Infrastructure.Services;

public class ForgotPasswordEmailSender : IEmailSender
{
    private readonly ISendGridClient _sendGridClient;
    private readonly string _senderEmail;
    private readonly string _senderName;
    
    public ForgotPasswordEmailSender(ISendGridClient sendGridClient, string senderEmail, string senderName)
    {
        _sendGridClient = sendGridClient;
        _senderEmail = senderEmail;
        _senderName = senderName;
    }

    public async Task<HttpStatusCode> SendEmailAsync(string email, object body)
    {
        var from = new EmailAddress(_senderEmail,_senderName);
        var to = new EmailAddress(email);
        var dynamicTemplateData = new
            {button_url = (string)body};
        var mail = MailHelper.CreateSingleTemplateEmail(from, to, "d-4c6e6d8cfac0420ca33afda75c53fef0", dynamicTemplateData);
        var response = await _sendGridClient.SendEmailAsync(mail);
        return response.StatusCode;
    }
}