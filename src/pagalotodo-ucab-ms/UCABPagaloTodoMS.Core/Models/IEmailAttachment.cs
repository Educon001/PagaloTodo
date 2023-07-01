namespace UCABPagaloTodoMS.Core.Models;

public interface IEmailAttachment
{
    string Name { get; }
    byte[] Data { get; }
}