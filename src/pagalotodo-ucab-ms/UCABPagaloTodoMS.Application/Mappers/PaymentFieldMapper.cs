using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public class PaymentFieldMapper
{
    public static PaymentFieldResponse MapEntityToResponse(PaymentFieldEntity entity)
    {
        var response = new PaymentFieldResponse
        {
            Name = entity.Name,
            Format = entity.Format
        };
        return response;
    }

    public static PaymentFieldEntity MapRequestToEntity(PaymentFieldRequest request, ServiceEntity serviceE)
    {
        var entity = new PaymentFieldEntity
        {
            Name = request.Name,
            Format = request.Format,
            Service = serviceE,
        };
        return entity;
    }
}