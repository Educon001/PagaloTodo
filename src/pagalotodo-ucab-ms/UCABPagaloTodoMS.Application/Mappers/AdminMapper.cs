using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public static class AdminMapper
{
    public static AdminResponse MapEntityToResponse(AdminEntity entity)
    {
        var response = new AdminResponse()
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            Email = entity.Email,
            Name = entity.Name, 
            LastName = entity.LastName,
            Status = entity.Status
        };
        return response;
    }

    public static AdminEntity MapRequestToEntity(AdminRequest request)
    {
        var entity = new AdminEntity()
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Status = request.Status
        };
        return entity;
    }
}