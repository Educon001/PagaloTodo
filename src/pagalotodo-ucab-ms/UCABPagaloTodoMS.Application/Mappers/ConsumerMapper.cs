using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Mappers;

public static class ConsumerMapper
{
    public static ConsumerResponse MapEntityToResponse(ConsumerEntity entity)
    {
        var response = new ConsumerResponse()
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            Email = entity.Email,
            Name = entity.Name,
            LastName = entity.LastName,
            Status = entity.Status,
            ConsumerId = entity.ConsumerId,
            Payments = entity.Payments?.Select(p => PaymentMapper.MapEntityToResponse(p, false, true)).ToList()
        };
        return response;
    }

    public static ConsumerEntity MapRequestToEntity(ConsumerRequest request)
    {
        var entity = new ConsumerEntity()
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash != null ? SecurePasswordHasher.Hash(request.PasswordHash) : null,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Status = request.Status,
            ConsumerId = request.ConsumerId
        };
        return entity;
    }
}