using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Mappers;

public class ServiceMapper
{
    public static ServiceResponse MapEntityToResponse(ServiceEntity entity, bool isProviderReference)
    {
        if (!isProviderReference)
        {
            entity.Provider!.Services = null;
        }
        var response = new ServiceResponse()
        {
            Id = entity.Id,
            Description = entity.Description,
            Name = entity.Name,
            ServiceType = entity.ServiceType.ToString(),
            ConfirmationList = entity.ConfirmationList?.Select(c => DebtorsMapper.MapEntityToResponse(c)).ToList(),
            ServiceStatus = entity.ServiceStatus.ToString(),
            Provider = isProviderReference ? null : ProviderMapper.MapEntityToResponse(entity.Provider!),
            Payments = entity.Payments?.Select(p=>PaymentMapper.MapEntityToResponse(p,true, false)).ToList(),
            ConciliationFormat = entity.ConciliationFormat?.Select(f=>FieldMapper.MapEntityToResponse(f)).ToList()
        };
        return response;
    }

    public static ServiceEntity MapRequestToEntity(ServiceRequest request, ProviderEntity providerE)
    {
        ServiceEntity entity = new(){
            Description = request.Description,
            Name = request.Name,
            ServiceType = request.ServiceType ?? 0,
            ServiceStatus = request.ServiceStatus ?? 0,
            Provider = providerE
        };
        return entity;
    }
}