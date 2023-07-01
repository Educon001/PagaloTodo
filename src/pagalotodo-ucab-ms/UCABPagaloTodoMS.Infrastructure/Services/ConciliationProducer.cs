using System.Text;
using RabbitMQ.Client;
using UCABPagaloTodoMS.Core.Services;

namespace UCABPagaloTodoMS.Infrastructure.Services;

public class ConciliationProducer : IRabbitMqProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public ConciliationProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "conciliation_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public void PublishMessage(byte[] message)
    {
        _channel.BasicPublish(exchange: string.Empty,
            routingKey: "conciliation_queue",
            basicProperties: null,
            body: message);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}