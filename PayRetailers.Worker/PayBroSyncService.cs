using MassTransit;
using PayRetailers.Application.External;
using PayRetailers.Contracts.PayBro;
using PayRetailers.Domain.Entities;

namespace PayRetailers.Worker;
public class PayBroSyncService(IRequestClient<GetPayBroAccountsRequest> client)
    : IPayBroSyncService
{
    public async Task<IReadOnlyCollection<PayBroAccount>> GetAllAccountsAsync(CancellationToken ct = default)
    {
        var response = await client.GetResponse<PayBroAccountsResponse>(new GetPayBroAccountsRequest(), ct);

        return response.Message.Accounts;
    }
}
