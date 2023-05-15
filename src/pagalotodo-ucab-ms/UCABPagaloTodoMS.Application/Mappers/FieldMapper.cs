using MassTransit;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers;

public class FieldMapper
{
    public static FieldResponse MapEntityToResponse(FieldEntity entity)
    {
        var response = new FieldResponse()
        {
            Id = entity.Id,
            Name = entity.Name,
            Format = entity.Format,
            Length = entity.Length,
            Type = entity.Type,
            AttrReference = entity.AttrReference
        };
        return response;
    }

    public static FieldEntity MapRequestToEntity(FieldRequest request, IUCABPagaloTodoDbContext dbContext)
    {
        var entity = new FieldEntity()
        {
            Name = request.Name,
            Format = request.Format,
            Length = request.Length,
            Service = dbContext.Services.Find(request.Service),
            Type = request.Type,
            AttrReference = request.AttrReference
        };
        return entity;
    }
}