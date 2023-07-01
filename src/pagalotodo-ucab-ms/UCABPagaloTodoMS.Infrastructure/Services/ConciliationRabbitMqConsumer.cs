using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoMS.Core.Services;
using UCABPagaloTodoMS.Infrastructure.Database;

namespace UCABPagaloTodoMS.Infrastructure.Services;

public class ConciliationRabbitMqConsumer : BackgroundService
{
    private readonly DbContextFactory _dbContextFactory;
    private IConnection _connection;
    private IModel _channel;

    public ConciliationRabbitMqConsumer(DbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        var factory = new ConnectionFactory {HostName = "localhost"};
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "conciliation_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            await UpdatePaymentsStatus(body);
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume(queue: "conciliation_queue", autoAck: false, consumer: consumer);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private async Task UpdatePaymentsStatus(byte[] body)
    {
        using var dbContext = _dbContextFactory.CreateDbContext(null!);
        using var stream = new MemoryStream(body);
        using var reader = new StreamReader(stream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        var idIndex = csv.GetFieldIndex("Id Pago");
        var statusIndex = csv.GetFieldIndex("Confirmacion");

        var idList = new List<Guid>();
        var statusList = new List<string>();

        while (csv.Read())
        {
            idList.Add(csv.GetField<Guid>(idIndex));
            statusList.Add(csv.GetField<string>(statusIndex)!);
        }

        for (int i = 0; i < idList.Count; i++)
        {
            var transaccion = dbContext.BeginTransaction();
            try
            {
                var entity = await dbContext.Payments.FindAsync(idList[i]);
                switch (statusList[i].ToUpper()[0])
                {
                    case 'S':
                        entity!.PaymentStatus = PaymentStatusEnum.Aprovado;
                        break;

                    case 'N':
                        entity!.PaymentStatus = PaymentStatusEnum.Rechazado;
                        break;

                    default:
                        entity!.PaymentStatus = PaymentStatusEnum.Pendiente;
                        break;
                }

                dbContext.Payments.Update(entity);
                await dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
            }
            catch (Exception)
            {
                transaccion.Rollback();
            }
        }
    }
    
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}