using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public static class DebtorsMapper
{
    public static DebtorsResponse MapEntityToResponse(DebtorsEntity entity)
    {
        var response = new DebtorsResponse()
        {
            Id = entity.Id,
            Identifier = entity.Identifier,
            Amount = entity.Amount,
            Status = entity.Status
        };
        return response;
    }

    public static DebtorsEntity MapRequestToEntity(DebtorsRequest request)
    {
        var entity = new DebtorsEntity()
        {
            Identifier = request.Identifier,
            Amount = request.Amount,
            Status = request.Status
        };
        return entity;
    }
}