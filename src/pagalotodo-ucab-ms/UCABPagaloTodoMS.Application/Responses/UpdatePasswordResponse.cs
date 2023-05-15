namespace UCABPagaloTodoMS.Application.Responses;

public class UpdatePasswordResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public UpdatePasswordResponse(Guid id, string message)
    {
        Id = id;
        Message = message;
    }
}