using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

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
            Services = new List<ServiceResponse>()
        };
        return response;
    }

    public static ProviderEntity MapRequestToEntity(ProviderRequest request)
    {
        var entity = new ProviderEntity()
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash,
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