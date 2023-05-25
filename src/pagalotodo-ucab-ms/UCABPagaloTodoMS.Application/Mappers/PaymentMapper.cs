
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public class PaymentMapper
{
    public static PaymentResponse MapEntityToResponse(PaymentEntity entity, bool isServiceReference, bool isConsumerReference)
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
            Amount = entity.Amount,
            Identifier = entity.Identifier,
            OriginAccount = entity.OriginAccount,
            PaymentStatus = entity.PaymentStatus,
            Service = isServiceReference? null : ServiceMapper.MapEntityToResponse(entity.Service!,false),
            Consumer = isConsumerReference? null : ConsumerMapper.MapEntityToResponse(entity.Consumer!)
        };
        return response;
    }

    public static PaymentEntity MapRequestToEntity(PaymentRequest request, ServiceEntity serviceE, ConsumerEntity consumerE)
    {
        var entity = new PaymentEntity()
        {
            Amount = request.Amount,
            Identifier = request.Identifier,
            OriginAccount = request.OriginAccount,
            PaymentStatus = request.PaymentStatus,
            Service = serviceE,
            Consumer = consumerE
        };
        return entity;
    }
}