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
}