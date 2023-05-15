using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Debtors;

public class GetDebtorsByServiceIdQuery : IRequest<List<DebtorsResponse>>
{
    public Guid Id { get; set; }

    public GetDebtorsByServiceIdQuery(Guid id)
    {
        Id = id;
    }
}