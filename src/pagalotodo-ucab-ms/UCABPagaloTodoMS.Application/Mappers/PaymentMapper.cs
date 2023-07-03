using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public class PaymentMapper
{
    public static PaymentResponse MapEntityToResponse(PaymentEntity entity, bool isServiceReference,
        bool isConsumerReference)
    {
        if (!isConsumerReference)
        {
            entity.Consumer!.Payments = null;
        }

        if (!isServiceReference)
        {
            entity.Service!.Payments = null;
        }

        var response = new PaymentResponse()
        {
            Id = entity.Id,
            PaymentDate = entity.CreatedAt.ToLocalTime(),
            Amount = entity.Amount,
            Identifier = entity.Identifier,
            CardholderName = entity.CardholderName,
            CardNumber = new string('*', entity.CardNumber!.Length - 4) +
                         entity.CardNumber.Substring(entity.CardNumber.Length - 4),
            TransactionId = entity.TransactionId,
            PaymentStatus = entity.PaymentStatus,        
            Service = isServiceReference ? null : ServiceMapper.MapEntityToResponse(entity.Service!, false),
            Consumer = isConsumerReference ? null : ConsumerMapper.MapEntityToResponse(entity.Consumer!),
            PaymentDetails = entity?.PaymentDetails.Select(f=>PaymentDetailMapper.MapEntityToResponse(f)).ToList()
        };
        return response;
    }

    public static PaymentEntity MapRequestToEntity(PaymentRequest request, ServiceEntity serviceE,
        ConsumerEntity consumerE)
    {
        var entity = new PaymentEntity()
        {
            Amount = request.Amount,
            Identifier = request.Identifier,
            CardholderName = request.CardholderName,
            CardNumber = request.CardNumber?.Replace("-","").Replace(" ",""),
            ExpirationMonth = request.ExpirationMonth,
            ExpirationYear = request.ExpirationYear<100? request.ExpirationYear+2000 : request.ExpirationYear,
            CardSecurityCode = request.CardSecurityCode,
            PaymentStatus = request.PaymentStatus,
            Service = serviceE,
            Consumer = consumerE,
            TransactionId = Guid.NewGuid().ToString(),
        };
        return entity;
    }
}