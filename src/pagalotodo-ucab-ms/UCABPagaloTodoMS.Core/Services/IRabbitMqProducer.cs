namespace UCABPagaloTodoMS.Core.Services;

public interface IRabbitMqProducer
{
    void PublishMessage(byte[] message);
}

public delegate IRabbitMqProducer ProducerResolver(string key);