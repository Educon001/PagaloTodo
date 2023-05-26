﻿using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Requests;

public class PaymentRequest
{
    public float? Amount { get; set; }
    public string? OriginAccount { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public Guid? Consumer { get; set; }
    public Guid? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}