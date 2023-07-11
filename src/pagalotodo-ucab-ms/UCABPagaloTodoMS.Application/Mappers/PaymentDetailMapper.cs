using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public class PaymentDetailMapper
{
    public static PaymentDetailResponse MapEntityToResponse(PaymentDetailEntity entity)
    {
        var response = new PaymentDetailResponse
        {
            Name = entity.Name,
            Value = entity.Value
        };
        return response;
    }

    public static PaymentDetailEntity MapRequestToEntity(PaymentDetailRequest request, PaymentEntity paymentE)
    {
        var entity = new PaymentDetailEntity
        {
            Name = request.Name,
            Value = request.Value,
            Payment = paymentE
        };
        return entity;
    }
}