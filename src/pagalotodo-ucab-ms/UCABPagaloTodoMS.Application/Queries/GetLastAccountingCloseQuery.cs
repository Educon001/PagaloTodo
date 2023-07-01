using MediatR;

namespace UCABPagaloTodoMS.Application.Queries;

public class GetLastAccountingCloseQuery : IRequest<DateTime>
{
}