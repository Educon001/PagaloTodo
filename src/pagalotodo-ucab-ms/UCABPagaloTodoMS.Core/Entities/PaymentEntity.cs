﻿using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Core.Entities;

public class PaymentEntity : BaseEntity
{
    public float? Amount { get; set; }
    public string? OriginAccount { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerEntity? Consumer { get; set; }
    public ServiceEntity? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}