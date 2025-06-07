using MassTransit;
using PayRetailers.Application.External;
using PayRetailers.Contracts.PayBro;

namespace PayRetailers.Worker;
public class ProviderRequestConsumer(
    IPayBroClient payBroClient,
    IPublishEndpoint publish) : IConsumer<GetPayBroAccountsRequest>
{
    public async Task Consume(ConsumeContext<GetPayBroAccountsRequest> context)
    {
        var accounts = await payBroClient.GetAccountsAsync();
        await publish.Publish(new PayBroAccountsResponse(accounts));
    }
}
