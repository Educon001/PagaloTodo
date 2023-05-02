﻿using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Entities;

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
            FullName = entity.Name + ' ' + entity.LastName,
            Status = entity.Status,
            ConsumerId = entity.ConsumerId,
            Payments = new List<PaymentResponse>()
        };
        return response;
    }

    public static ConsumerEntity MapRequestToEntity(ConsumerRequest request)
    {
        var entity = new ConsumerEntity()
        {
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            Status = request.Status,
            ConsumerId = request.ConsumerId
        };
        return entity;
    }
}