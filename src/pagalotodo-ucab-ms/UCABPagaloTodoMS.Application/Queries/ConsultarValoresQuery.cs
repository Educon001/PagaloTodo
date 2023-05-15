using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class ConsultarValoresQuery : IRequest<List<ServiceResponse>>
{
    public ConsultarValoresQuery()
    {
        
    }
}