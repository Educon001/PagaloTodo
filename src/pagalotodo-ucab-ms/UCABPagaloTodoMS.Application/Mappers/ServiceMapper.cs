using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Mappers;

public class ServiceMapper
{
    public static ServiceResponse MapEntityToResponse(ServiceEntity entity)
    {
        var response = new ServiceResponse()
        {
            Id = entity.Id,
            Description = entity.Description,
            Name = entity.Name,
            ServiceType = entity.ServiceType,
            ConfirmationList = entity.ServiceType==ServiceTypeEnum.Directo? new List<DebtorsResponse>() : null,
            ServiceStatus = entity.ServiceStatus,
            Provider = entity.Provider!=null? ProviderMapper.MapEntityToResponse(entity.Provider) : null,
            Payments = new List<PaymentResponse>(),
            ConciliationFormat = new List<FieldResponse>()
        };
        return response;
    }

    public static ServiceEntity MapRequestToEntity(ServiceRequest request)
    {
        var entity = new ServiceEntity()
        {
            Description = request.Description,
            Name = request.Name,
            ServiceType = request.ServiceType,
            ServiceStatus = request.ServiceStatus,
            Provider = request.Provider!=null? ProviderMapper.MapRequestToEntity(request.Provider) : null
        };
        return entity;
    }
}