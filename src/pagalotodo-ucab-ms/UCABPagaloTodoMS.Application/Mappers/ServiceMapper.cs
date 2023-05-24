using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Mappers;

public class ServiceMapper
{
    public static ServiceResponse MapEntityToResponse(ServiceEntity entity)
    {
        // List<FieldResponse>? conciliationList = new List<FieldResponse>()!;
        // foreach (var fEntity in entity.ConciliationFormat!)
        // {
        //     conciliationList.Add(FieldMapper.MapEntityToResponse(fEntity));
        // }
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

    public static ServiceEntity MapRequestToEntity(ServiceRequest request, ProviderEntity providerE)
    {
        ServiceEntity entity = new(){
            Description = request.Description,
            Name = request.Name,
            ServiceType = request.ServiceType,
            ServiceStatus = request.ServiceStatus,
            Provider = providerE
        };
        return entity;
    }
}