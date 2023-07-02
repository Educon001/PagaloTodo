using MediatR;

namespace UCABPagaloTodoMS.Application.Requests;

public class UploadDebtorsRequest : IRequest<bool>
{
    public byte[] Data { get; set; }
    public Guid ServiceId { get; set; }

    public UploadDebtorsRequest(byte[] data, Guid serviceId)
    {
        Data = data;
        ServiceId = serviceId;
    }
}