using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoMS.Core.Services;
using UCABPagaloTodoMS.Infrastructure.Database;

namespace UCABPagaloTodoMS.Infrastructure.Services;

public class ConfirmationListRabbitMqConsumer : BackgroundService
{
    private readonly DbContextFactory _dbContextFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public ConfirmationListRabbitMqConsumer(DbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        var factory = new ConnectionFactory() {HostName = "localhost"};
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "confirmation_list_queue",
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
            var byteId = new byte[16];
            var csv = new byte[body.Length - 16];
            Array.Copy(body,0,byteId,0,16);
            Array.Copy(body,16,csv,0,csv.Length);
            await AddConfirmationList(csv,new Guid(byteId));
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume(queue: "confirmation_list_queue", autoAck: false, consumer: consumer);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private async Task AddConfirmationList(byte[] csvData, Guid serviceId)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var transaccion = dbContext.BeginTransaction();
        try
        {
            var oldDebtors = dbContext.Debtors.Where(d=>d.Service!.Id==serviceId);
            dbContext.Debtors.RemoveRange(oldDebtors);
            await dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
        }
        catch (Exception)
        {
            transaccion.Rollback();
        }
        
        using var stream = new MemoryStream(csvData);
        using var reader = new StreamReader(stream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();

        var identifierList = new List<string>();
        var amountList = new List<double>();

        while (csv.Read())
        {
            identifierList.Add(csv.GetField<string>(0)!);
            amountList.Add(csv.GetField<double>(1));
        }

        var serviceEntity = await dbContext.Services.FindAsync(serviceId);
        for (int i = 0; i < identifierList.Count; i++)
        {
            transaccion = dbContext.BeginTransaction();
            try
            {
                var entity = new DebtorsEntity()
                {
                    Identifier = identifierList[i],
                    Amount = amountList[i],
                    Service = serviceEntity,
                    Status = false
                };
                dbContext.Debtors.Add(entity);
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