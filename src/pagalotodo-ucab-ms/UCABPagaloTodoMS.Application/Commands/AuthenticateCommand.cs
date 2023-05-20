using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands;

public class AuthenticateCommand : IRequest<LoginResponse>
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}