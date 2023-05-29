using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Requests;

public class ForgotPasswordRequest : IRequest<ForgotPasswordResponse>
{
    public string? UserEmail { get; set; }
    public string? Url { get; set; }

    public ForgotPasswordRequest(string? userEmail, string? url)
    {
        UserEmail = userEmail;
        Url = url;
    }
}