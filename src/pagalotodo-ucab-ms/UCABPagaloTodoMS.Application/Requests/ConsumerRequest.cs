namespace UCABPagaloTodoMS.Application.Requests;

public class ConsumerRequest : UserRequest
{
    public string? LastName { get; set; }
    public string? ConsumerId { get; set; }
}