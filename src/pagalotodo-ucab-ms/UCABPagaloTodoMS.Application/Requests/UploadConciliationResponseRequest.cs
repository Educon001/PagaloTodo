using MediatR;

namespace UCABPagaloTodoMS.Application.Requests;

public class UploadConciliationResponseRequest : IRequest<bool>
{
    public byte[] Data { get; set; }

    public UploadConciliationResponseRequest(byte[] data)
    {
        Data = data;
    }
}