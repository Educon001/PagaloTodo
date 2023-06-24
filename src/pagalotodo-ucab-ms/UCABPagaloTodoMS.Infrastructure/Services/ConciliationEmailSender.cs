using System.Net;
using Microsoft.Azure.Amqp.Serialization;
using SendGrid;
using SendGrid.Helpers.Mail;
using UCABPagaloTodoMS.Core.Models;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Infrastructure.Services;

public class ConciliationEmailSender : IEmailSender
{
    private readonly ISendGridClient _sendGridClient;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public ConciliationEmailSender(ISendGridClient sendGridClient, string senderEmail, string senderName)
    {
        _sendGridClient = sendGridClient;
        _senderEmail = senderEmail;
        _senderName = senderName;
    }

    public async Task<HttpStatusCode> SendEmailAsync(string email, object body)
    {
        var from = new EmailAddress(_senderEmail, _senderName);
        var to = new EmailAddress(email);
        var mail = MailHelper.CreateSingleEmail(from, to, "PagaloTodo - Archivos de Conciliación",
            "Marqué el campo de confirmación de cada pago con una S(SI) o una N(NO) para conciliar sus pagos recibidos",
            null);
        dynamic list = body;
        var fileList = new List<IEmailAttachment>();
        foreach (dynamic item in list)
        {
            fileList.Add((IEmailAttachment) item);
        }

        foreach (var file in fileList)
        {
            var attachment = new Attachment
            {
                Filename = file.Name,
                Content = Convert.ToBase64String(file.Data),
                Type = "text/csv"
            };
            mail.AddAttachment(attachment);
        }

        var response = await _sendGridClient.SendEmailAsync(mail);
        return response.StatusCode;
    }
}