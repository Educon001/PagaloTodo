using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries
{
    public class ConsultarValoresPruebaQuery : IRequest<List<ValoresResponse>>
    { }
}

