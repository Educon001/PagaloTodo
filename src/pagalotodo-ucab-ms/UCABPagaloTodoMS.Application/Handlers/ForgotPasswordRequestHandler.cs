using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Application.Handlers;

public class ForgotPasswordRequestHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ForgotPasswordRequestHandler> _logger;

    public ForgotPasswordRequestHandler(IEmailSender emailSender, IUCABPagaloTodoDbContext dbContext,
        ILogger<ForgotPasswordRequestHandler> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.UserEmail is null || request.Url is null)
            {
                _logger.LogWarning("ForgotPasswordRequestHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    public async Task<ForgotPasswordResponse> HandleAsync(ForgotPasswordRequest request)
    {
        var response = await _emailSender.SendEmailAsync(request.UserEmail!, request.Url!);
        if (response == HttpStatusCode.Accepted)
        {
            return new ForgotPasswordResponse(ForgotPasswordResponse.Successful);
        }
        return new ForgotPasswordResponse(ForgotPasswordResponse.Unsuccessful);
    }
}