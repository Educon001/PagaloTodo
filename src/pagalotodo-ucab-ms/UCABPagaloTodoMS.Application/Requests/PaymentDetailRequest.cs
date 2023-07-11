using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Requests;

public class PaymentDetailRequest
{
    public string? Name { get; set; }

    public string? Value { get; set; }
    
    public Guid? Payment { get; set; }
}