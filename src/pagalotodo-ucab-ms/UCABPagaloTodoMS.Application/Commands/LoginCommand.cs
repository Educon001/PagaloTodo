using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands;

public class LoginCommand : IRequest<LoginResponse>
{
    public LoginRequest Request { get; set; }

    public LoginCommand(LoginRequest request)
    {
        Request = request;
    }
}