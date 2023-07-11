using MediatR;

namespace UCABPagaloTodoMS.Application.Requests;

public class UploadDebtorsRequest : IRequest<bool>
{
    public byte[] Data { get; }
    public Guid ServiceId { get; }

    public UploadDebtorsRequest(byte[] data, Guid serviceId)
    {
        Data = data;
        ServiceId = serviceId;
    }
}