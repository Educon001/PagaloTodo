using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Mappers
{
    public static class ValoresMapper
    {
        public static ValoresResponse MapEntityAResponse(ValoresEntity entity)
        {
            var response = new ValoresResponse()
            {
                Id = entity.Id,
                Nombre = entity.Nombre + entity.Apellido,
                Identificacion = entity.Identificacion
            };
            return response;    
        }

        public static ValoresEntity MapRequestEntity(ValoresRequest request)
        {
            var entity = new ValoresEntity()
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Identificacion = request.Identificacion ?? string.Empty
            };
            return entity;
        }
    }
}
