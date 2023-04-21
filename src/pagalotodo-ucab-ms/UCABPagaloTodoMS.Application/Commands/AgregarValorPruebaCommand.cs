using MediatR;

namespace UCABPagaloTodoMS.Application.Commands
{
    public class AgregarValorPruebaCommand : IRequest<List<string>>
    {
        public string valor { get; set; }

        public AgregarValorPruebaCommand(string request)
        {
            valor = request;
        }
    }
}
