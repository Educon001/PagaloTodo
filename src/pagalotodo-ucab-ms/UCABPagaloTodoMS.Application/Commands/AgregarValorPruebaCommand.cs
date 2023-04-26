using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands
{
    public class AgregarValorPruebaCommand : IRequest<Guid>
    {
        public ValoresRequest _request { get; set; }

        public AgregarValorPruebaCommand(ValoresRequest request)
        {
            _request = request;
        }
    }
}
