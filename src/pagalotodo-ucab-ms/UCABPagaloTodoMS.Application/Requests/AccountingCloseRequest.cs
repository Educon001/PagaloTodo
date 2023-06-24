using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Requests;

public class AccountingCloseRequest : IRequest<string>
{
    public List<ProviderResponse> Providers { get; set; }

    public AccountingCloseRequest(List<ProviderResponse> providers)
    {
        Providers = providers;
    }
}