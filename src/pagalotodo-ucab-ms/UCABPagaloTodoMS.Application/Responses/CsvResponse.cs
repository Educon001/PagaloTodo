using UCABPagaloTodoMS.Core.Models;

namespace UCABPagaloTodoMS.Application.Responses;

public class CsvResponse : IEmailAttachment
{
    public string Name { get; }
    public byte[] Data { get; }

    public CsvResponse(string name, byte[] file)
    {
        Name = name;
        Data = file;
    }
}