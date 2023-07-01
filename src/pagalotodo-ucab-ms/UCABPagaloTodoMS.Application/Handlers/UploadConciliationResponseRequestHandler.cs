using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Application.Handlers;

public class UploadConciliationResponseRequestHandler : IRequestHandler<UploadConciliationResponseRequest, bool>
{
    private readonly ILogger<UploadConciliationResponseRequestHandler> _logger;
    private readonly IRabbitMqProducer _producer;

    public UploadConciliationResponseRequestHandler(ILogger<UploadConciliationResponseRequestHandler> logger,
        ProducerResolver resolver)
    {
        _logger = logger;
        _producer = resolver("Conciliation");
    }

    public Task<bool> Handle(UploadConciliationResponseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _producer.PublishMessage(request.Data);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
}