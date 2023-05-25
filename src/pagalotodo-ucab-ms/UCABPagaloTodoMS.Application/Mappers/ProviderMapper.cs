using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Mappers;

public class ProviderMapper
{
    public static ProviderResponse MapEntityToResponse(ProviderEntity entity)
    {
        var response = new ProviderResponse()
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            Email = entity.Email,
            FullName = entity.Name + ' ' + entity.LastName,
            Status = entity.Status,
            Rif = entity.Rif,
            AccountNumber = entity.AccountNumber,
            Services = entity.Services?.Select(s=>ServiceMapper.MapEntityToResponse(s,true)).ToList()
        };
        return response;
    }

    public static ProviderEntity MapRequestToEntity(ProviderRequest request)
    {
        var entity = new ProviderEntity()
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash!=null? SecurePasswordHasher.Hash(request.PasswordHash) : null,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Status = request.Status,
            Rif = request.Rif,
            AccountNumber = request.AccountNumber
        };
        return entity;
    }
}