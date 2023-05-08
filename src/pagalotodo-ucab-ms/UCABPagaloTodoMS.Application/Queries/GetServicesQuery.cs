using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class GetServicesQuery : IRequest<List<ServiceResponse>>
{

}