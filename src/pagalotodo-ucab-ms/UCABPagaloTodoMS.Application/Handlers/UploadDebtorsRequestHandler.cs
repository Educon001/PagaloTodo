using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Application.Handlers;

public class UploadDebtorsRequestHandler : IRequestHandler<UploadDebtorsRequest, bool>
{
    private readonly ILogger<UploadDebtorsRequestHandler> _logger;
    private readonly IRabbitMqProducer _producer;

    public UploadDebtorsRequestHandler(ILogger<UploadDebtorsRequestHandler> logger,
        ProducerResolver resolver)
    {
        _logger = logger;
        _producer = resolver("Confirmation");
    }
    
    public Task<bool> Handle(UploadDebtorsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var body = new byte[request.Data.Length + 16];
            Array.Copy(request.ServiceId.ToByteArray(), 0, body, 0, 16);
            Array.Copy(request.Data, 0, body, 16, request.Data.Length);
            _producer.PublishMessage(body);
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            return Task.FromResult(false);
        }
    }
}